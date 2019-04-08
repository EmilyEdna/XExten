using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using XExten.XCore;
using XExten.DynamicType;
using System.Reflection;
using System.Data;
using System.ComponentModel;

namespace XExten.XExpres
{
    /// <summary>
    /// Expression Extension Class
    /// </summary>
    public class XExp
    {
        #region Func
        private static readonly Dictionary<DateTime, Object> CacheData = new Dictionary<DateTime, Object>();
        private static readonly IDictionary<String, Dictionary<DateTime, Object>> CacheObject = new Dictionary<String, Dictionary<DateTime, Object>>();
        private static readonly IDictionary<String, Dictionary<DateTime, Object>> CacheBool = new Dictionary<String, Dictionary<DateTime, Object>>();
        /// <summary>
        ///  Return AttributeType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Express"></param>
        /// <returns></returns>
        public static T GetAttributeType<T, K>(Expression<Func<K, Object>> Express)
        {
            if (Express == null) return default(T);
            MemberExpression Exp = (MemberExpression)Express.Body;
            var Attribute = (T)Exp.Member.GetCustomAttributes(typeof(T), true).FirstOrDefault();
            return Attribute;
        }
        /// <summary>
        /// Set Properties Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonValue"></param>
        /// <param name="Param"></param>
        public static void SetProptertiesValue<T>(Dictionary<String, Object> JsonValue, T Param) where T : class, new()
        {
            var type = typeof(T);
            foreach (var NameValue in JsonValue)
            {
                var property = type.GetProperty(NameValue.Key);

                var objectParameterExpression = Expression.Parameter(typeof(Object), "obj");
                var objectUnaryExpression = Expression.Convert(objectParameterExpression, type);

                var valueParameterExpression = Expression.Parameter(typeof(Object), "val");
                var valueUnaryExpression = Expression.Convert(valueParameterExpression, property.PropertyType);

                // 调用给属性赋值的方法
                var body = Expression.Call(objectUnaryExpression, property.GetSetMethod(), valueUnaryExpression);
                var expression = Expression.Lambda<Action<T, Object>>(body, objectParameterExpression, valueParameterExpression);

                var Actions = expression.Compile();
                Actions(Param, NameValue.Value);
            };
        }
        /// <summary>
        ///  Return a new expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, Object>> GetExpression<T>(params string[] PropertyName) where T : class, new()
        {
            List<MemberBinding> Exps = new List<MemberBinding>();
            ParameterExpression Parameter = Expression.Parameter(typeof(T), "t");
            PropertyName.ByEach<String>(Item =>
            {
                MemberExpression PropertyExpress = Expression.Property(Parameter, Item);
                UnaryExpression ConvterExpress = Expression.Convert(PropertyExpress, typeof(T).GetProperty(Item).PropertyType);
                Exps.Add(Expression.Bind(typeof(T).GetProperty(Item), PropertyExpress));
            });
            MemberInitExpression Member = Expression.MemberInit(Expression.New(typeof(T)), Exps);
            return Expression.Lambda<Func<T, Object>>(Member, Parameter);
        }
        /// <summary>
        ///  return a bool expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Property"></param>
        /// <param name="Data"></param>
        /// <param name="QueryType"></param>
        /// <returns></returns>
        public static Expression<Func<T, Boolean>> GetExpression<T>(String Property, Object Data, QType QueryType)
        {
            ParameterExpression Parameter = Expression.Parameter(typeof(T), "t");
            if (typeof(T).GetProperty(Property) == null)
            {
                throw new MissingFieldException("Field not found,please Check");
            }
            MemberExpression Left = Expression.Property(Parameter, typeof(T).GetProperty(Property));
            ConstantExpression Right = Expression.Constant(Data, Data.GetType());
            Expression Filter = null;
            switch (QueryType)
            {
                case QType.Like:
                    Filter = Expression.Call(Left, typeof(String).GetMethod("Contains", new Type[] { typeof(String) }), Right);
                    break;
                case QType.NotLike:
                    Filter = Expression.Not(Expression.Call(Left, typeof(String).GetMethod("Contains", new Type[] { typeof(String) }), Right));
                    break;
                case QType.Equals:
                    Filter = Expression.Equal(Left, Right);
                    break;
                case QType.NotEquals:
                    Filter = Expression.NotEqual(Left, Right);
                    break;
                case QType.GreaterThan:
                    Filter = Expression.GreaterThan(Left, Right);
                    break;
                case QType.GreaterThanOrEqual:
                    Filter = Expression.GreaterThanOrEqual(Left, Right);
                    break;
                case QType.LessThan:
                    Filter = Expression.LessThan(Left, Right);
                    break;
                case QType.LessThanOrEqual:
                    Filter = Expression.LessThanOrEqual(Left, Right);
                    break;
                default:
                    Filter = Expression.Equal(Left, Right);
                    break;
            }
            return Expression.Lambda<Func<T, bool>>(Filter, Parameter);
        }
        /// <summary>
        /// Combine two classes into one class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Express"></param>
        /// <returns></returns>
        public static Type CombineClass<T, K>(Expression<Func<T, K, Object>> Express)
        {
            List<DynamicProperty> dynamics = new List<DynamicProperty>();
            (Express.Body as NewExpression).Arguments.ByEachs(item =>
            {
                var type = (item as ParameterExpression).Type;
                type.GetProperties().ByEachs(t =>
                {
                    DynamicProperty dynamic = new DynamicProperty(t.Name, t.PropertyType);
                    if (!dynamics.Contains(dynamic))
                        dynamics.Add(dynamic);
                });
            });
            return DynamicClassBuilder.Instance.GetDynamicClass(dynamics, "DynamicClass");
        }
        /// <summary>
        /// Combine two classes into one class with value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Express"></param>
        /// <param name="dynamics"></param>
        public static Object CombineClassWithValue<T, K>(Expression<Func<T, K, Object>> Express, List<DynamicPropertyValue> dynamics)
        {
            var DynamicType = Activator.CreateInstance(CombineClass(Express));
            dynamics.ForEach(item =>
            {
                if (item.Type == DynamicType.GetType().GetProperty(item.Name).PropertyType)
                    DynamicType.GetType().GetProperty(item.Name).SetValue(DynamicType, item.Value);
            });
            return DynamicType;
        }
        /// <summary>
        /// Creating data reports through expressions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Queryable"></param>
        /// <param name="GroupName"></param>
        /// <param name="Express"></param>
        /// <returns></returns>
        public static DataTable CreateTable<T>(IQueryable<T> Queryable, Expression<Func<T, String>> GroupName, params Expression<Func<T, double>>[] Express)
        {
            DataTable table = new DataTable();
            MemberExpression memberExpression = GroupName.Body as MemberExpression;
            var displayName = (memberExpression.Member.GetCustomAttributes(false).FirstOrDefault() as DescriptionAttribute).Description;
            table.Columns.Add(new DataColumn(displayName));
            foreach (var expression in Express)
            {
                memberExpression = expression.Body as MemberExpression;
                displayName = (memberExpression.Member.GetCustomAttributes(false).FirstOrDefault() as DescriptionAttribute).Description;
                table.Columns.Add(new DataColumn(displayName));
            }
            var groups = Queryable.GroupBy(GroupName);
            foreach (var group in groups)
            {
                //  Message：设置分组列头
                DataRow dataRow = table.NewRow();
                dataRow[0] = group.Key;
                //  Message：设置分组汇总数据
                for (int i = 0; i < Express.Length; i++)
                {
                    var expression = Express[i];
                    Func<T, double> fun = expression.Compile();
                    dataRow[i + 1] = group.Sum(fun);
                }
                table.Rows.Add(dataRow);
            }
            return table;
        }
        /// <summary>
        ///  Cache Object Expression 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Express"></param>
        /// <param name="CacheKey"></param>
        /// <param name="CacheMilliseconds"></param>
        /// <returns></returns>
        public static void CacheExpression<T>(Expression<Func<T, Object>> Express, String CacheKey, int CacheMilliseconds = 0)
        {
            if (CacheMilliseconds == 0)
                CacheData.Add(DateTime.Parse(DateTime.Now.ToShortDateString()), Express);
            else
                CacheData.Add(DateTime.Now.AddMilliseconds(CacheMilliseconds), Express);
            CacheObject.Add(CacheKey, CacheData);
        }
        /// <summary>
        /// Cache bool Expression 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Express"></param>
        /// <param name="CacheKey"></param>
        /// <param name="CacheMilliseconds"></param>
        public static void CacheExpression<T>(Expression<Func<T, bool>> Express, String CacheKey, int CacheMilliseconds = 0)
        {
            if (CacheMilliseconds == 0)
                CacheData.Add(DateTime.Parse(DateTime.Now.ToShortDateString()), Express);
            else
                CacheData.Add(DateTime.Now.AddMilliseconds(CacheMilliseconds), Express);
            CacheBool.Add(CacheKey, CacheData);
        }
        /// <summary>
        ///  Get Object Expression Cache 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static Expression<Func<T, Object>> GetObjectCache<T>(String CacheKey)
        {
            foreach (var Item in CacheObject)
            {
                foreach (var Keys in Item.Value)
                {
                    if (Keys.Key < DateTime.Parse(DateTime.Now.ToShortDateString()))
                    {
                        Item.Value.Remove(Keys.Key);
                        CacheObject.Remove(Item.Key);
                    }
                    else if(Keys.Key < DateTime.Now)
                    {
                        Item.Value.Remove(Keys.Key);
                        CacheObject.Remove(Item.Key);
                    }
                }
            }
            return (Expression<Func<T, Object>>)CacheObject[CacheKey].Values.FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetBoolCache<T>(String CacheKey)
        {
            foreach (var Item in CacheBool)
            {
                foreach (var Keys in Item.Value)
                {
                    if (Keys.Key < DateTime.Parse(DateTime.Now.ToShortDateString()))
                    {
                        Item.Value.Remove(Keys.Key);
                        CacheBool.Remove(Item.Key);
                    }
                    else if (Keys.Key < DateTime.Now)
                    {
                        Item.Value.Remove(Keys.Key);
                        CacheBool.Remove(Item.Key);
                    }
                }
            }
            return (Expression<Func<T, bool>>)CacheBool[CacheKey].Values.FirstOrDefault();
        }
        #endregion
    }
}
