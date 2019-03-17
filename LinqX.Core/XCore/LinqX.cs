using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LinqX.Core.XCore
{
    public static class LinqX
    {
        public static T ToUTF8<T>(this T Param) where T : class
        {
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                var data = t.GetValue(Param).ToString();
                if (!string.IsNullOrEmpty(data))
                {
                    var bytes = Encoding.Unicode.GetBytes(data);
                    StringBuilder str = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i += 2)
                    {
                        str.AppendFormat("u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
                    }
                    t.SetValue(Param, str.ToString());
                }
            });
            return Param;
        }
        public static T ToUnic<T>(this T Param) where T : class
        {
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                var data = t.GetValue(Param).ToString();
                if (!string.IsNullOrEmpty(data))
                {
                    var result = new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                    .Replace(data, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
                    t.SetValue(Param, result);
                }
            });
            return Param;
        }
        public static List<String> ToNames<T>(this T Param) where T : class
        {
            List<String> Names = new List<String>();
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                Names.Add(t.Name);
            });
            return Names;
        }
        public static List<Object> ToValues<T>(this T Param) where T : class
        {
            List<Object> Values = new List<Object>();
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                Values.Add(t.GetValue(Param));
            });
            return Values;
        }
        public static IDictionary<String, Object> ToDic<T>(this T Param) where T : class
        {
            ParameterExpression Parameter = Expression.Parameter(Param.GetType(), "t");
            Dictionary<String, Object> Map = new Dictionary<String, Object>();
            Param.GetType().GetProperties().ToList().ForEach(item =>
            {
                MemberExpression PropertyExpress = Expression.Property(Parameter, item);
                UnaryExpression ConvterExpress = Expression.Convert(PropertyExpress, typeof(object));
                Func<T, Object> GetValueFunc = Expression.Lambda<Func<T, object>>(ConvterExpress, Parameter).Compile();
                Map.Add(item.Name, GetValueFunc(Param));
            });
            return Map;
        }
        public static String ToDes<T>(this T Param, Expression<Func<T, object>> Express) where T : class
        {
            MemberExpression Exp = (MemberExpression)Express.Body;
            var Obj = Exp.Member.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            return (Obj as DescriptionAttribute).Description;
        }
        public static Int64 ToLong<T>(this T Param, Expression<Func<T, object>> Express) where T : class
        {
            String Str = ((Express.Body as MemberExpression).Member as PropertyInfo).GetValue(Param).ToString();
            Int64.TryParse(Str, out Int64 Value);
            return Value;
        }
        public static void ToSet<T>(this T Param, Expression<Func<T, object>> Expres, Object Value) where T : class
        {
            var Property = ((Expres.Body as MemberExpression).Member as PropertyInfo);

            var objectParameterExpression = Expression.Parameter(typeof(object), "obj");
            var objectUnaryExpression = Expression.Convert(objectParameterExpression, typeof(T));

            var valueParameterExpression = Expression.Parameter(typeof(object), "val");
            var valueUnaryExpression = Expression.Convert(valueParameterExpression, Property.PropertyType);
            // 调用给属性赋值的方法
            var body = Expression.Call(objectUnaryExpression, Property.GetSetMethod(), valueUnaryExpression);
            var expression = Expression.Lambda<Action<T, object>>(body, objectParameterExpression, valueParameterExpression);
            var Actions = expression.Compile();
            Actions(Param, Value);
        }
    }
}
