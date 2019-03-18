using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
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
        #region SyncByMap
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
        public static T ByUnic<T>(this T Param) where T : new()
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
        public static T ByUTF8<T>(this T Param) where T : new()
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
        public static Func<T, K> Funcs<T, K>() where T : new() where K :new()
        {
            var SourceType = typeof(T);
            var TargetType = typeof(K);
            ParameterExpression parameter = Expression.Parameter(SourceType, "t");
            List<MemberBinding> members = new List<MemberBinding>();
            foreach (var sourceItem in SourceType.GetProperties())
            {
                var targetItem = TargetType.GetProperty(sourceItem.Name);
                if (targetItem == null || sourceItem.PropertyType != targetItem.PropertyType)
                    continue;
                if (sourceItem == null || !sourceItem.CanRead || sourceItem.PropertyType.IsNotPublic)
                    continue;
                if (sourceItem.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;
                var property = Expression.Property(parameter, sourceItem);
                //当非值类型且类型不相同时
                if (!sourceItem.PropertyType.IsValueType && sourceItem.PropertyType != targetItem.PropertyType)
                {
                    //判断都是(非泛型、非数组)class
                    if (sourceItem.PropertyType.IsClass && targetItem.PropertyType.IsClass
                        && !sourceItem.PropertyType.IsArray && !targetItem.PropertyType.IsArray
                        && !sourceItem.PropertyType.IsGenericType && !targetItem.PropertyType.IsGenericType)
                    {
                        var expression = GetClassExpression(property, sourceItem.PropertyType, targetItem.PropertyType);
                        members.Add(Expression.Bind(targetItem, expression));
                    }

                    //集合数组类型的转换
                    if (typeof(IEnumerable).IsAssignableFrom(sourceItem.PropertyType) && typeof(IEnumerable).IsAssignableFrom(targetItem.PropertyType))
                    {
                        var expression = GetListExpression(property, sourceItem.PropertyType, targetItem.PropertyType);
                        members.Add(Expression.Bind(targetItem, expression));
                    }
                    continue;
                }
                //可空类型转换到非可空类型，当可空类型值为null时，用默认值赋给目标属性；不为null就直接转换
                if (IsNullableType(sourceItem.PropertyType) && !IsNullableType(targetItem.PropertyType))
                {
                    var hasValueExpression = Expression.Equal(Expression.Property(property, "HasValue"), Expression.Constant(true));
                    var conditionItem = Expression.Condition(hasValueExpression, Expression.Convert(property, targetItem.PropertyType), Expression.Default(targetItem.PropertyType));
                    members.Add(Expression.Bind(targetItem, conditionItem));
                    continue;
                }

                //非可空类型转换到可空类型，直接转换
                if (!IsNullableType(sourceItem.PropertyType) && IsNullableType(targetItem.PropertyType))
                {
                    var memberExpression = Expression.Convert(property, targetItem.PropertyType);
                    members.Add(Expression.Bind(targetItem, memberExpression));
                    continue;
                }

                if (targetItem.PropertyType != sourceItem.PropertyType)
                    continue;


                var memberBinding = Expression.Bind(targetItem, property);
                members.Add(memberBinding);
            }
            //创建一个if条件表达式
            var test = Expression.NotEqual(parameter, Expression.Constant(null, SourceType));// p==null;
            var ifTrue = Expression.MemberInit(Expression.New(SourceType), members);
            var condition = Expression.Condition(test, ifTrue, Expression.Constant(null, SourceType));
            var lambda=  Expression.Lambda<Func<T, K>>(condition, parameter);
            return lambda.Compile();
        }
        public static K ByMap<T, K>(this T Param) where T : new() where K : new()
        {
            var p = Funcs<T, K>();
            return p(Param);
        }
        public static IEnumerable<K> ByMaps<T, K>(this IEnumerable<T> queryable) where T : new() where K : new()
        {
            var p = Funcs<T, K>();
            return queryable.Select(p);
        }
        /// <summary>
        ///  return  a list with this T's PropertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static List<String> ByNames<T>(this T Param) where T : new()
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
        public static List<Object> ByValues<T>(this T Param) where T : new()
        {
            List<Object> Values = new List<Object>();
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                Values.Add(t.GetValue(Param));
            });
            return Values;
        }
        /// <summary>
        /// return a Dictionary with this T's PropertyName and PropertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static IDictionary<String, Object> ByDic<T>(this T Param) where T : new()
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
        ///  return bool
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
        public static String ByDes<T>(this T Param, Expression<Func<T, object>> Expres) where T : new()
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
        public static Int64 ByLong<T>(this T Param, Expression<Func<T, object>> Expres) where T : new()
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
        public static void BySet<T>(this T Param, Expression<Func<T, object>> Expres, Object Value) where T : new()
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
        ///  return  table
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
        public static async Task<T> ByUnicAsync<T>(this T Param) where T : new()
        {
            return await Task.Run(() => ByUnic(Param));
        }
        /// <summary>
        /// return this T with replace value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<T> ByUTF8Async<T>(this T Param) where T : new()
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
        //public static async  Task<K> ByMapAsync<T, K>(this T Param) where T : new()
        //{
        //    return await Task.Run(() => ByMap<T, K>(Param));
        //}
        /// <summary>
        /// return  a list with this T's PropertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<List<String>> ByNamesAsync<T>(this T Param) where T : new()
        {
            return await Task.Run(() => ByNames(Param));
        }
        /// <summary>
        /// return a List with this T's PropertyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<List<Object>> ByValuesAsync<T>(this T Param) where T : new()
        {
            return await Task.Run(() => ByValues(Param));
        }
        /// <summary>
        /// return bool
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
        public static async Task<IDictionary<String, Object>> ByDicAsync<T>(this T Param) where T : new()
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
        public static async Task<String> ByDesAsync<T>(this T Param, Expression<Func<T, object>> Expres) where T : new()
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
        public static async Task<Int64> ByLongAsync<T>(this T Param, Expression<Func<T, object>> Expres) where T : new()
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
        public static async Task BySetAsync<T>(this T Param, Expression<Func<T, object>> Expres, Object Value) where T : new()
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
        ///  return  table
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
        #endregion

        /// <summary>
        /// 类型是clas时赋值
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="targetProperty"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static Expression GetClassExpression(Expression sourceProperty, Type sourceType, Type targetType)
        {
            //条件p.Item!=null
            var testItem = Expression.NotEqual(sourceProperty, Expression.Constant(null, sourceType));

            //构造回调 Mapper<TSource, TTarget>.Map()
            var mapperType = typeof(LinqX).GetMethod("ByMap", new[] { sourceType });
            var iftrue = Expression.Call(mapperType, sourceProperty);

            var conditionItem = Expression.Condition(testItem, iftrue, Expression.Constant(null, targetType));

            return conditionItem;
        }
        /// <summary>
        /// 类型为集合时赋值
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="targetProperty"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static Expression GetListExpression(Expression sourceProperty, Type sourceType, Type targetType)
        {
            //条件p.Item!=null
            var testItem = Expression.NotEqual(sourceProperty, Expression.Constant(null, sourceType));

            //构造回调 Mapper<TSource, TTarget>.MapList()
            var sourceArg = sourceType.IsArray ? sourceType.GetElementType() : sourceType.GetGenericArguments()[0];
            var targetArg = targetType.IsArray ? targetType.GetElementType() : targetType.GetGenericArguments()[0];
            var mapperType = typeof(LinqX).GetMethod("ByMaps", new[] { sourceType });

            var mapperExecMap = Expression.Call(mapperType, sourceProperty);

            Expression iftrue;
            if (targetType == mapperExecMap.Type)
            {
                iftrue = mapperExecMap;
            }
            else if (targetType.IsArray)//数组类型调用ToArray()方法
            {
                iftrue = Expression.Call(typeof(Enumerable), nameof(Enumerable.ToArray), new[] { mapperExecMap.Type.GenericTypeArguments[0] }, mapperExecMap);
            }
            else if (typeof(IDictionary).IsAssignableFrom(targetType))
            {
                iftrue = Expression.Constant(null, targetType);//字典类型不转换
            }
            else
            {
                iftrue = Expression.Convert(mapperExecMap, targetType);
            }

            var conditionItem = Expression.Condition(testItem, iftrue, Expression.Constant(null, targetType));

            return conditionItem;
        }
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
