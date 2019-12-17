using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.HttpFactory
{
    /// <summary>
    /// 封装数据
    /// </summary>
    public class HttpKeyPairs
    {
        /// <summary>
        /// 创建一个key-value格式的表单数据(Making form data to KeyValuePairs)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <param name="Map"></param>
        /// <returns></returns>
        public static List<KeyValuePair<String, String>> KeyValuePairs<T>(T Entity, IDictionary<string, string> Map = null) where T : class, new()
        {
            List<KeyValuePair<String, String>> keyValuePairs = new List<KeyValuePair<string, string>>();
            Entity.GetType().GetProperties().ToList().ForEach(t =>
            {
                var flag = t.CustomAttributes.Where(x => x.AttributeType == typeof(JsonIgnoreAttribute)).FirstOrDefault();
                if (Map != null)
                    foreach (KeyValuePair<String, String> item in Map)
                    {
                        if (item.Key.Equals(t.Name))
                            keyValuePairs.Add(new KeyValuePair<string, string>(item.Value, t.GetValue(Entity).ToString()));
                    }
                else if (flag == null)
                    keyValuePairs.Add(new KeyValuePair<string, string>(t.Name, t.GetValue(Entity).ToString()));
            });
            return keyValuePairs;
        }
    }
}
