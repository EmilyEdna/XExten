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
        ///  return a unicode string
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
        ///  return a uft8 string
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
        /// return this T with replace value
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
        ///  return this T with replace value
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
        /// return another type
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
        ///  Foreach the array
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
        ///  return  a list with this T's PropertyName
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
        ///  return a List with this T's PropertyValue
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
        ///  return another type
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
        /// return a Dictionary with this T's PropertyName and PropertyValue
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
        ///  Check IEnumerable is nullable and return true or false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> queryable)
        {
            return queryable == null || !queryable.Any();
        }
        /// <summary>
        ///  return DescriptionAttribute value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static String ByDes<T>(this T Param, Expression<Func<T, object>> Expres)
        {
            MemberExpression Exp = (MemberExpression)Expres.Body;
            var Obj = Exp.Member.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            return (Obj as DescriptionAttribute).Description;
        }
        /// <summary>
        ///  return Long type with Convert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static Int64 ByLong<T>(this T Param, Expression<Func<T, object>> Expres)
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
        public static void BySet<T>(this T Param, Expression<Func<T, object>> Expres, Object Value)
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
        ///  return pagination
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
        ///  return table
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
        /// <param name="transform"></param>
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
        ///  foreach the dictionary
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
        #endregion

        #region Async
        /// <summary>
        /// return a unicode string
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ByUnicAsync(this String Param)
        {
            return await Task.Run(() => ByUnic(Param));
        }
        /// <summary>
        /// return a uft8 string
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ByUTF8Async(this String Param)
        {
            return await Task.Run(() => ByUTF8(Param));
        }
        /// <summary>
        /// return this T with replace value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<T> ByUnicAsync<T>(this T Param)
        {
            return await Task.Run(() => ByUnic(Param));
        }
        /// <summary>
        /// return this T with replace value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<T> ByUTF8Async<T>(this T Param)
        {
            return await Task.Run(() => ByUTF8(Param));
        }
        /// <summary>
        ///  return another type
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
        /// Foreach the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static async Task ByEachAsync<T>(this Array Param, Action<T> Selector)
        {
            await Task.Run(() => ByEach(Param, Selector));
        }
        /// <summary>
        /// return  a list with this T's PropertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<List<String>> ByNamesAsync<T>(this T Param)
        {
            return await Task.Run(() => ByNames(Param));
        }
        /// <summary>
        /// return a List with this T's PropertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<List<Object>> ByValuesAsync<T>(this T Param)
        {
            return await Task.Run(() => ByValues(Param));
        }
        /// <summary>
        ///  return another type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<K>> ByMapsAsync<T, K>(this IEnumerable<T> queryable)
        {
            return await Task.Run(() => ByMaps<T, K>(queryable));
        }
        /// <summary>
        /// Check IEnumerable is nullable and return true or false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static async Task<Boolean> IsNullOrEmptyAsync<T>(this IEnumerable<T> queryable)
        {
            return await Task.Run(() => IsNullOrEmpty(queryable));
        }
        /// <summary>
        /// return a Dictionary with this T's PropertyName and PropertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<IDictionary<String, Object>> ByDicAsync<T>(this T Param)
        {
            return await Task.Run(() => ByDic(Param));
        }
        /// <summary>
        /// return DescriptionAttribute value
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
        ///  return Long type with Convert
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
        ///  return pagination
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
        ///  return table
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
        ///  foreach the dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        public static async Task ByDicEachAsync<T, K>(this IDictionary<T, K> Param, Action<T, K> Selector)
        {
            await Task.Run(() => ByDicEach(Param, Selector));
        }
        #endregion

        #region Expres
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
        #endregion
    }
}
