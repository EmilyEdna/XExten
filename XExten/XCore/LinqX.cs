using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XExten.XCore
{
    public static class LinqX
    {
        #region Func
        private static Func<T, K> Funcs<T, K>()
        {
            var SType = typeof(T);
            var TType = typeof(K);
            if (IsEnumerable(SType) || IsEnumerable(TType))
                throw new NotSupportedException("Enumerable types are not supported,please use ByMaps method.");
            ParameterExpression Parameter = Expression.Parameter(SType, "t");
            List<MemberBinding> Bindings = new List<MemberBinding>();
            var TTypes = TType.GetProperties().Where(x => x.PropertyType.IsPublic && x.CanWrite);
            foreach (var TItem in TTypes)
            {
                PropertyInfo SItem = SType.GetProperty(TItem.Name);
                //check Model can write or read
                if (SItem == null || !SItem.CanRead || SItem.PropertyType.IsNotPublic)
                    continue;
                //ignore map 
                if (SItem.GetCustomAttribute<IgnoreMappedAttribute>() != null)
                    continue;
                MemberExpression SProperty = Expression.Property(Parameter, SItem);
                if (!SItem.PropertyType.IsValueType && SItem.PropertyType != SItem.PropertyType)
                {
                    //is not GenericType and Array
                    if (SItem.PropertyType.IsClass && TItem.PropertyType.IsClass
                        && !SItem.PropertyType.IsArray && !TItem.PropertyType.IsArray
                        && !SItem.PropertyType.IsGenericType && !TItem.PropertyType.IsGenericType)
                    {
                        Expression Exp = GetClassExpression(SProperty, SItem.PropertyType, TItem.PropertyType);
                        Bindings.Add(Expression.Bind(TItem, Exp));
                    }
                    //IEnumerable Convter
                    if (typeof(IEnumerable).IsAssignableFrom(SItem.PropertyType) && typeof(IEnumerable).IsAssignableFrom(TItem.PropertyType))
                    {
                        Expression Exp = GetListExpression(SProperty, SItem.PropertyType, TItem.PropertyType);
                        Bindings.Add(Expression.Bind(TItem, Exp));
                    }
                    continue;
                }
                //可空类型转换到非可空类型，当可空类型值为null时，用默认值赋给目标属性；不为null就直接转换
                if (IsNullableType(SItem.PropertyType) && !IsNullableType(TItem.PropertyType))
                {
                    BinaryExpression BinaryItem = Expression.Equal(Expression.Property(SProperty, "HasValue"), Expression.Constant(true));
                    ConditionalExpression CItem = Expression.Condition(BinaryItem, Expression.Convert(SProperty, TItem.PropertyType), Expression.Default(TItem.PropertyType));
                    Bindings.Add(Expression.Bind(TItem, CItem));
                    continue;
                }
                //非可空类型转换到可空类型，直接转换
                if (!IsNullableType(SItem.PropertyType) && IsNullableType(TItem.PropertyType))
                {
                    UnaryExpression Unary = Expression.Convert(SProperty, TItem.PropertyType);
                    Bindings.Add(Expression.Bind(TItem, Unary));
                    continue;
                }
                if (TItem.PropertyType != SItem.PropertyType)
                    continue;
                Bindings.Add(Expression.Bind(TItem, SProperty));
            }
            //创建一个if条件表达式
            BinaryExpression Binary = Expression.NotEqual(Parameter, Expression.Constant(null, SType));
            MemberInitExpression Member = Expression.MemberInit(Expression.New(TType), Bindings);
            ConditionalExpression Condition = Expression.Condition(Binary, Member, Expression.Constant(null, TType));
            return Expression.Lambda<Func<T, K>>(Condition, Parameter).Compile();
        }
        private static Expression GetClassExpression(Expression SProperty, Type SType, Type TType)
        {
            var Item = Expression.NotEqual(SProperty, Expression.Constant(null, SType));
            //构造回调 Mapper<TSource, TTarget>.Map()
            var MType = typeof(LinqX).GetMethod("ByMap", new[] { SType });
            var Call = Expression.Call(MType, SProperty);
            return Expression.Condition(Item, Call, Expression.Constant(null, TType));
        }
        private static Expression GetListExpression(Expression SProperty, Type SType, Type TType)
        {
            //条件p.Item!=null
            var Item = Expression.NotEqual(SProperty, Expression.Constant(null, SType));
            var MType = typeof(LinqX).GetMethod("ByMaps", new[] { SType });
            var Call = Expression.Call(MType, SProperty);
            Expression Exp;
            if (TType == Call.Type)
                Exp = Call;
            else if (TType.IsArray)//数组类型调用ToArray()方法
                Exp = Expression.Call(typeof(Enumerable), nameof(Enumerable.ToArray), new[] { Call.Type.GenericTypeArguments[0] }, Call);
            else if (typeof(IDictionary).IsAssignableFrom(TType))
                Exp = Expression.Constant(null, TType);//字典类型不转换
            else
                Exp = Expression.Convert(Call, TType);
            return Expression.Condition(Item, Exp, Expression.Constant(null, TType));
        }
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        private static bool IsEnumerable(Type type)
        {
            return type.IsArray || type.GetInterfaces().Any(x => x == typeof(ICollection) || x == typeof(IEnumerable));
        }
        #endregion

        #region Sync
        /// <summary>
        /// Return Unicode string
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static String ByUnic(this String Param)
        {
            if (String.IsNullOrEmpty(Param))
                return String.Empty;
            else
            {
                var bytes = Encoding.Unicode.GetBytes(Param);
                StringBuilder str = new StringBuilder();
                for (int i = 0; i < bytes.Length; i += 2)
                {
                    str.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
                }
                return str.ToString();
            }
        }
        /// <summary>
        ///  Return UTF8 string
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static String ByUTF8(this String Param)
        {
            if (String.IsNullOrEmpty(Param))
                return String.Empty;
            else
                return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                      .Replace(Param, x => String.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }
        /// <summary>
        /// Replace the data in the entity and return it as Unicode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static T ByUnic<T>(this T Param)
        {
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                var data = t.GetValue(Param).ToString();
                if (!String.IsNullOrEmpty(data))
                {
                    var bytes = Encoding.Unicode.GetBytes(data);
                    StringBuilder str = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i += 2)
                    {
                        str.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
                    }
                    t.SetValue(Param, str.ToString());
                }
            });
            return Param;
        }
        /// <summary>
        ///  Replace the data in the entity and return it as UTF8
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static T ByUTF8<T>(this T Param)
        {
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                var data = t.GetValue(Param).ToString();
                if (!String.IsNullOrEmpty(data))
                {
                    var result = new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                    .Replace(data, x => String.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
                    t.SetValue(Param, result);
                }
            });
            return Param;
        }
        /// <summary>
        /// Map an entity to another entity and return the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static K ByMap<T, K>(this T Param)
        {
            return (Funcs<T, K>())(Param);
        }
        /// <summary>
        /// Traversing the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        public static void ByEach<T>(this Array Param, Action<T> Selector)
        {
            foreach (var item in Param)
                Selector((T)item);
        }
        /// <summary>
        ///  Traverse collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="Selector"></param>
        public static void ByEachs<T>(this IEnumerable<T> queryable, Action<T> Selector)
        {
            foreach (var item in queryable)
                Selector((T)item);
        }
        /// <summary>
        ///  Returns all Property names in an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static List<String> ByNames<T>(this T Param)
        {
            List<String> Names = new List<String>();
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                Names.Add(t.Name);
            });
            return Names;
        }
        /// <summary>
        ///  Returns all Property Values in an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static List<Object> ByValues<T>(this T Param)
        {
            List<Object> Values = new List<Object>();
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                Values.Add(t.GetValue(Param));
            });
            return Values;
        }
        /// <summary>
        /// Convert the collection to a data table and return the data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static DataTable ByTables<T>(this IList<T> queryable)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo item in typeof(T).GetProperties())
            {
                Type property = item.PropertyType;
                if ((property.IsGenericType) && (property.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    property = property.GetGenericArguments()[0];
                }
                dt.Columns.Add(new DataColumn(item.Name, property));
            }
            //创建数据行
            if (queryable.Count > 0)
            {
                for (int i = 0; i < queryable.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo item in typeof(T).GetProperties())
                    {
                        object obj = item.GetValue(queryable[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
        /// <summary>
        /// Convert the entity to a data table and return the data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static DataTable ByTable<T>(this T Param)
        {
            DataTable dt = new DataTable();
            ArrayList Temp = new ArrayList();
            Param.GetType().GetProperties().ByEach<PropertyInfo>(t =>
            {
                Type type = t.PropertyType;
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    type = type.GetGenericArguments()[0];
                dt.Columns.Add(new DataColumn(t.Name, type));
                Temp.Add(t.GetValue(Param));
                dt.LoadDataRow(Temp.ToArray(), true);
            });
            return dt;
        }
        /// <summary>
        ///  Map a collection to another collection and return the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IEnumerable<K> ByMaps<T, K>(this IEnumerable<T> queryable)
        {
            return queryable.Select(Funcs<T, K>());
        }
        /// <summary>
        /// Wraps an entity's property name and property value traversal into the dictionary and returns the dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static IDictionary<String, Object> ByDic<T>(this T Param)
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
        /// <summary>
        ///  Determine if the collection is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> queryable)
        {
            return queryable == null || !queryable.Any();
        }
        /// <summary>
        ///  Returns an entity with a property value marked to describe the property field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static String ByDes<T>(this T Param, Expression<Func<T, Object>> Expres)
        {
            MemberExpression Exp = (MemberExpression)Expres.Body;
            var Obj = Exp.Member.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            return (Obj as DescriptionAttribute).Description;
        }
        /// <summary>
        ///  Convert a field type to a long integer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static Int64 ByLong<T>(this T Param, Expression<Func<T, Object>> Expres)
        {
            String Str = ((Expres.Body as MemberExpression).Member as PropertyInfo).GetValue(Param).ToString();
            Int64.TryParse(Str, out Int64 Value);
            return Value;
        }
        /// <summary>
        ///  set a value for T's Property which choose
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <param name="Value"></param>
        public static void BySet<T>(this T Param, Expression<Func<T, Object>> Expres, Object Value)
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
        /// <summary>
        ///  Return paging data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static Page<T> ByPage<T>(this IEnumerable<T> queryable, int PageIndex, int PageSize)
        {
            return new Page<T>
            {
                Total = queryable.Count(),
                TotalPage = (int)Math.Ceiling(queryable.Count() / (double)PageSize),
                CurrentPage = (int)Math.Ceiling(PageIndex / (double)PageSize) + 1,
                Queryable = queryable.Skip((PageIndex - 1) * PageSize).Take(PageSize).AsQueryable()
            };
        }
        /// <summary>
        ///  return  data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static DataTable ByTable<T>(this IEnumerable<T> queryable, int PageIndex, int PageSize)
        {
            var properties = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(properties.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            queryable = queryable.Skip((PageIndex - 1) * PageSize).Take(PageSize);
            if (queryable.Count() > 0)
            {
                for (int i = 0; i < queryable.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo property in properties)
                    {
                        object obj = property.GetValue(queryable.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
        /// <summary>
        /// Transform your shit into some other shit.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="MapForm"></param>
        /// <returns></returns>
        public static IEnumerable<K> BySend<T, K>(this IEnumerable<T> queryable, Func<T, K> MapForm)
        {
            if (queryable == null || MapForm == null) throw new ArgumentNullException();
            var iterator = queryable.GetEnumerator();
            while (iterator.MoveNext())
            {
                yield return MapForm(iterator.Current);
            }
        }
        /// <summary>
        ///  Traversing the dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        public static void ByDicEach<T, K>(this IDictionary<T, K> Param, Action<T, K> Selector)
        {
            foreach (KeyValuePair<T, K> item in Param)
                Selector(item.Key, item.Value);
        }
        /// <summary>
        /// Returns all values of a field in a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static IEnumerable<Object> ByOver<T>(this IEnumerable<T> queryable, Expression<Func<T, Object>> Expres)
        {
            PropertyInfo property = (Expres.Body as MemberExpression).Member as PropertyInfo;
            IList<Object> Data = new List<Object>();
            queryable.ByEachs(t =>
            {
                Object value = t.GetType().GetProperty(property.Name).GetValue(t);
                Data.Add(value);
            });
            return Data;
        }
        /// <summary>
        /// Convert a data table to an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static T ByEntity<T>(this DataTable Param) where T : new()
        {
            T entity = new T();
            foreach (DataRow row in Param.Rows)
            {
                foreach (var item in entity.GetType().GetProperties())
                {
                    if (row.Table.Columns.Contains(item.Name))
                        if (DBNull.Value != row[item.Name])
                        {
                            Type newType = item.PropertyType;
                            if (newType.IsGenericType && newType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                NullableConverter nullableConverter = new NullableConverter(newType);
                                newType = nullableConverter.UnderlyingType;
                            }
                            item.SetValue(entity, Convert.ChangeType(row[item.Name], newType), null);
                        }
                }
            }
            return entity;
        }
        /// <summary>
        /// Convert a data table to entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static IList<T> ByEntities<T>(this DataTable Param) where T : new()
        {
            IList<T> entities = new List<T>();
            if (Param == null)
                return null;
            foreach (DataRow row in Param.Rows)
            {
                T entity = new T();
                foreach (var item in entity.GetType().GetProperties())
                {
                    if (Param.Columns.Contains(item.Name))
                        if (DBNull.Value != row[item.Name])
                        {
                            Type newType = item.PropertyType;
                            if (newType.IsGenericType && newType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                NullableConverter nullableConverter = new NullableConverter(newType);
                                newType = nullableConverter.UnderlyingType;
                            }
                            item.SetValue(entity, Convert.ChangeType(row[item.Name], newType), null);
                        }
                }
                entities.Add(entity);
            }
            return entities;
        }
        #endregion

        #region Async
        /// <summary>
        /// Return Unicode string
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ByUnicAsync(this String Param)
        {
            return await Task.Run(() => ByUnic(Param));
        }
        /// <summary>
        ///  Return UTF8 string
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ByUTF8Async(this String Param)
        {
            return await Task.Run(() => ByUTF8(Param));
        }
        /// <summary>
        /// Replace the data in the entity and return it as Unicode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<T> ByUnicAsync<T>(this T Param)
        {
            return await Task.Run(() => ByUnic(Param));
        }
        /// <summary>
        ///  Replace the data in the entity and return it as UTF8
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<T> ByUTF8Async<T>(this T Param)
        {
            return await Task.Run(() => ByUTF8(Param));
        }
        /// <summary>
        /// Map an entity to another entity and return the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<K> ByMapAsync<T, K>(this T Param)
        {
            return await Task.Run(() => ByMap<T, K>(Param));
        }
        /// <summary>
        /// Traversing the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        public static async Task ByEachAsync<T>(this Array Param, Action<T> Selector)
        {
            await Task.Run(() => ByEach(Param, Selector));
        }
        /// <summary>
        ///  Traverse collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="Selector"></param>
        public static async Task ByEachsAsync<T>(this IEnumerable<T> queryable, Action<T> Selector)
        {
            await Task.Run(() => ByEachs(queryable, Selector));
        }
        /// <summary>
        ///  Returns all Property names in an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<List<String>> ByNamesAsync<T>(this T Param)
        {
            return await Task.Run(() => ByNames(Param));
        }
        /// <summary>
        ///  Returns all Property Values in an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<List<Object>> ByValuesAsync<T>(this T Param)
        {
            return await Task.Run(() => ByValues(Param));
        }
        /// <summary>
        /// Convert the collection to a data table and return the data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static async Task<DataTable> ByTablesAsync<T>(this IList<T> queryable)
        {
            return await Task.Run(() => ByTables(queryable));
        }
        /// <summary>
        /// Convert the entity to a data table and return the data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<DataTable> ByTableAsync<T>(this T Param)
        {
            return await Task.Run(() => ByTable(Param));
        }
        /// <summary>
        ///  Map a collection to another collection and return the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<K>> ByMapsAsync<T, K>(this IEnumerable<T> queryable)
        {
            return await Task.Run(() => ByMaps<T, K>(queryable));
        }
        /// <summary>
        /// Wraps an entity's property name and property value traversal into the dictionary and returns the dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<IDictionary<String, Object>> ByDicAsync<T>(this T Param)
        {
            return await Task.Run(() => ByDic(Param));
        }
        /// <summary>
        ///  Determine if the collection is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static async Task<Boolean> IsNullOrEmptyAsync<T>(this IEnumerable<T> queryable)
        {
            return await Task.Run(() => IsNullOrEmpty(queryable));
        }
        /// <summary>
        ///  Returns an entity with a property value marked to describe the property field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static async Task<String> ByDesAsync<T>(this T Param, Expression<Func<T, object>> Expres)
        {
            return await Task.Run(() => ByDes(Param, Expres));
        }
        /// <summary>
        ///  Convert a field type to a long integer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static async Task<Int64> ByLongAsync<T>(this T Param, Expression<Func<T, object>> Expres)
        {
            return await Task.Run(() => ByLong(Param, Expres));
        }
        /// <summary>
        ///  set a value for T's Property which choose
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <param name="Value"></param>
        public static async Task BySetAsync<T>(this T Param, Expression<Func<T, object>> Expres, Object Value)
        {
            await Task.Run(() => BySet(Param, Expres, Value));
        }
        /// <summary>
        ///  Return paging data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static async Task<Page<T>> ByPageAsync<T>(this IEnumerable<T> queryable, int PageIndex, int PageSize)
        {
            return await Task.Run(() => ByPage(queryable, PageIndex, PageSize));
        }
        /// <summary>
        ///  return  data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static async Task<DataTable> ByTableAsync<T>(this IEnumerable<T> queryable, int PageIndex, int PageSize)
        {
            return await Task.Run(() => ByTable(queryable, PageIndex, PageSize));
        }
        /// <summary>
        /// Transform your shit into some other shit.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="MapForm"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<K>> BySendAsync<T, K>(this IEnumerable<T> queryable, Func<T, K> MapForm)
        {
            return await Task.Run(() => BySend(queryable, MapForm));
        }
        /// <summary>
        ///  Traversing the dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        public static async Task ByDicEachAsync<T, K>(this IDictionary<T, K> Param, Action<T, K> Selector)
        {
            await Task.Run(() => ByDicEach(Param, Selector));
        }
        /// <summary>
        /// Returns all values of a field in a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Object>> ByOverAsync<T>(this IEnumerable<T> queryable, Expression<Func<T, Object>> Expres)
        {
            return await Task.Run(() => ByOver(queryable, Expres));
        }
        /// <summary>
        /// Convert a data table to an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async  Task<T> ByEntityAsync<T>(this DataTable Param) where T : new()
        {
            return await Task.Run(() => ByEntity<T>(Param));
        }
        /// <summary>
        /// Convert a data table to entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<IList<T>> ByEntitiesAsync<T>(this DataTable Param) where T : new()
        {
            return await Task.Run(() => ByEntities<T>(Param));
        }
        #endregion
    }
}
