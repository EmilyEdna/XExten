using AutoMapper;
using AutoMapper.Configuration;
using MessagePack;
using MessagePack.Resolvers;
using Newtonsoft.Json;
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
using XExten.Common;
using XExten.Encryption;

namespace XExten.XCore
{
    /// <summary>
    /// Linq Extension Class
    /// </summary>
    public static class LinqX
    {
        #region Func

        private static Func<T, K> Funcs<T, K>()
        {
            var SType = typeof(T);
            var TType = typeof(K);
            if (IsEnumerable(SType) || IsEnumerable(TType))
                throw new NotSupportedException("Enumerable types are not supported,please use ToMaps method.");
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
            var MType = typeof(LinqX).GetMethod("ToMapper", new[] { SType });
            var Call = Expression.Call(MType, SProperty);
            return Expression.Condition(Item, Call, Expression.Constant(null, TType));
        }

        private static Expression GetListExpression(Expression SProperty, Type SType, Type TType)
        {
            //条件p.Item!=null
            var Item = Expression.NotEqual(SProperty, Expression.Constant(null, SType));
            var MType = typeof(LinqX).GetMethod("ToMappers", new[] { SType });
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

        private static bool IsEnumerable(Type type)
        {
            return type.IsArray || type.GetInterfaces().Any(x => x == typeof(ICollection) || x == typeof(IEnumerable));
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        #endregion Func

        #region Sync

        /// <summary>
        /// 使用AutoMapper将实体映射到另一个实体并返回该实体(Map an entity to another entity and return the entity by AutoMapper)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ToAutoMapper<T>(this Object obj)
        {
            if (obj == null) return default;
            IMapper mapper = new MapperConfiguration(t => t.CreateMap(obj.GetType(), typeof(T))).CreateMapper();
            return mapper.Map<T>(obj);
        }

        /// <summary>
        ///  使用AutoMapper将实体映射到另一个实体并返回该实体(Map an entity to another entity and return the entity by AutoMapper)
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> ToAutoMapper<K,T>(this Object obj)
        {
            if (obj == null) return default;
            IMapper mapper = new MapperConfiguration(t => t.CreateMap(typeof(K), typeof(T))).CreateMapper();
            return mapper.Map<List<T>>(obj);
        }

        /// <summary>
        /// 使用AutoMapper将实体映射到另一个实体并返回该实体(Map an entity to another entity and return the entity by AutoMapper)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="IgnoreNames"></param>
        /// <returns></returns>
        public static T ToAutoMapperWithIgnore<T>(this Object obj, String IgnoreNames)
        {
            if (obj == null) return default(T);
            MapperConfigurationExpression expression = new MapperConfigurationExpression();
            IMappingExpression mapping = expression.CreateMap(obj.GetType(), typeof(T));
            if (IgnoreNames.Contains("|"))
                IgnoreNames.Split('|').ToList().ForEach(t => { mapping.ForMember(t, x => x.Ignore()); });
            else
                mapping.ForMember(IgnoreNames, x => x.Ignore());
            IMapper mapper = new MapperConfiguration(expression).CreateMapper();
            return mapper.Map<T>(obj);
        }

        /// <summary>
        ///  返回具有标记为描述属性字段的属性值的实体
        ///  (Returns an entity with a property value marked to describe the property field)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static String ToDes<T>(this T Param, Expression<Func<T, Object>> Expres)
        {
            MemberExpression Exp = (MemberExpression)Expres.Body;
            var Obj = Exp.Member.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            return (Obj as DescriptionAttribute).Description;
        }

        /// <summary>
        /// 将实体的属性名称和属性值遍历包含到字典中并返回字典
        /// (Wraps an entity's property name and property value traversal into the dictionary and returns the dictionary)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static Dictionary<String, Object> ToDic<T>(this T Param)
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
        ///  循环字典(Traversing the dictionary)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        public static void ToDicEach<T, K>(this IDictionary<T, K> Param, Action<T, K> Selector)
        {
            foreach (KeyValuePair<T, K> item in Param)
                Selector(item.Key, item.Value);
        }

        /// <summary>
        /// 循环数组(Traversing the array)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        public static void ToEach<T>(this Array Param, Action<T> Selector)
        {
            foreach (var item in Param)
                Selector((T)item);
        }

        /// <summary>
        ///  循环集合(Traverse collection)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="Selector"></param>
        public static void ToEachs<T>(this IEnumerable<T> queryable, Action<T> Selector)
        {
            foreach (var item in queryable)
                Selector((T)item);
        }

        /// <summary>
        /// 将数据表转换为实体(Convert a data table to entities)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static List<T> ToEntities<T>(this DataTable Param) where T : new()
        {
            List<T> entities = new List<T>();
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

        /// <summary>
        /// 将数据表转换为实体(Convert a data table to an entity)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataTable Param) where T : new()
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
        /// 序列化(SerializeObject)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static String ToJson<T>(this T Param)
        {
            return JsonConvert.SerializeObject(Param);
        }

        /// <summary>
        /// 使用Express将实体映射到另一个实体并返回该实体(Map an entity to another entity and return the entity by express)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static K ToMapper<T, K>(this T Param)
        {
            return (Funcs<T, K>())(Param);
        }

        /// <summary>
        ///  将集合映射到另一个集合并返回该集合(Map a collection to another collection and return the collection)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static List<K> ToMappers<T, K>(this IEnumerable<T> queryable)
        {
            return queryable.Select(Funcs<T, K>()).ToList();
        }

        /// <summary>
        /// 返序列化(DeserializeObject)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static T ToModel<T>(this String Param)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(Param) == null ? (T)JsonConvert.DeserializeObject(Param, typeof(T)) : JsonConvert.DeserializeObject<T>(Param);
            }
            catch (Exception)
            {
                throw new Exception("Deserialize Objcet Exception");
            }
        }

        /// <summary>
        /// 使用MsgPack序列化(DeserializeObject For MessagePack)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="IsPublic"></param>
        /// <returns></returns>
        public static Byte[] ToMsgByte<T>(this T Param, Boolean IsPublic = true)
        {
            MessagePackSerializerOptions Options;
            if (IsPublic)
                Options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
            else
                Options = MessagePackSerializerOptions.Standard.WithResolver(DynamicObjectResolverAllowPrivate.Instance);
            return MessagePackSerializer.Serialize<T>(Param, Options);
        }

        /// <summary>
        /// 使用MsgPack序列化为Json(DeserializeObject To Json For MessagePack)
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static String ToMsgJson(this Byte[] Param)
        {
            return MessagePackSerializer.SerializeToJson(Param);
        }

        /// <summary>
        /// 使用MsgPack反序列化(DeserializeObject For MessagePack)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="IsPublic"></param>
        /// <returns></returns>
        public static T ToMsgModel<T>(this Byte[] Param, Boolean IsPublic = true)
        {
            MessagePackSerializerOptions Options;
            if (IsPublic)
                Options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
            else
                Options = MessagePackSerializerOptions.Standard.WithResolver(DynamicObjectResolverAllowPrivate.Instance);
            return MessagePackSerializer.Deserialize<T>(Param, Options);
        }

        /// <summary>
        ///  返回实体中所有的字段名(Returns all Property names in an entity)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static List<String> ToNames<T>(this T Param)
        {
            List<String> Names = new List<String>();
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                Names.Add(t.Name);
            });
            return Names;
        }

        /// <summary>
        /// 返回集合中字段的所有值(Returns all values of a field in a collection)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static List<Object> ToOver<T>(this IEnumerable<T> queryable, Expression<Func<T, Object>> Expres)
        {
            PropertyInfo property = (Expres.Body as MemberExpression).Member as PropertyInfo;
            List<Object> Data = new List<Object>();
            queryable.ToEachs(t =>
            {
                Object value = t.GetType().GetProperty(property.Name).GetValue(t);
                Data.Add(value);
            });
            return Data;
        }

        /// <summary>
        ///  返回分页数据(Return paging data)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static PageResult<T> ToPage<T>(this IEnumerable<T> queryable, int PageIndex, int PageSize)
        {
            return new PageResult<T>
            {
                Total = queryable.Count(),
                TotalPage = (int)Math.Ceiling(queryable.Count() / (double)PageSize),
                CurrentPage = (int)Math.Ceiling(PageIndex / (double)PageSize),
                Queryable = queryable.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList()
            };
        }

        /// <summary>
        ///  为选择的T的属性设置一个值(set a value for T's Property which choose)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <param name="Value"></param>
        public static void ToSet<T>(this T Param, Expression<Func<T, Object>> Expres, Object Value)
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
        /// 将实体转换为数据表并返回数据表(Convert the entity to a data table and return the data table)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static DataTable ToTable<T>(this T Param)
        {
            DataTable dt = new DataTable();
            ArrayList Temp = new ArrayList();
            Param.GetType().GetProperties().ToEach<PropertyInfo>(t =>
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
        ///  返回数据表(return data table)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static DataTable ToTable<T>(this IEnumerable<T> queryable, int PageIndex, int PageSize)
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
        /// 将集合转换为数据表并返回数据表(Convert the collection to a data table and return the data table)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static DataTable ToTables<T>(this IList<T> queryable)
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
        /// 转换成Unicode(Return Unicode string)
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static String ToUnicode(this String Param)
        {
            if (String.IsNullOrEmpty(Param))
                return String.Empty;
            else
            {
                var Totes = Encoding.Unicode.GetBytes(Param);
                StringBuilder str = new StringBuilder();
                for (int i = 0; i < Totes.Length; i += 2)
                {
                    str.AppendFormat("\\u{0}{1}", Totes[i + 1].ToString("x").PadLeft(2, '0'), Totes[i].ToString("x").PadLeft(2, '0'));
                }
                return str.ToString();
            }
        }

        /// <summary>
        /// 替换实体中的数据并将其作为Unicode返回(Replace the data in the entity and return it as Unicode)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static T ToUnicode<T>(this T Param)
        {
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                var data = t.GetValue(Param).ToString();
                if (!String.IsNullOrEmpty(data))
                {
                    var Totes = Encoding.Unicode.GetBytes(data);
                    StringBuilder str = new StringBuilder();
                    for (int i = 0; i < Totes.Length; i += 2)
                    {
                        str.AppendFormat("\\u{0}{1}", Totes[i + 1].ToString("x").PadLeft(2, '0'), Totes[i].ToString("x").PadLeft(2, '0'));
                    }
                    t.SetValue(Param, str.ToString());
                }
            });
            return Param;
        }

        /// <summary>
        /// 转换成UTF8(Return UTF8 string)
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static String ToUTF8(this String Param)
        {
            if (String.IsNullOrEmpty(Param))
                return String.Empty;
            else
                return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                      .Replace(Param, x => String.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }

        /// <summary>
        ///  替换实体中的数据并将其作为UTF8返回(Replace the data in the entity and return it as UTF8)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static T ToUTF8<T>(this T Param)
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
        /// 返回实体中所有的字段值(Returns all Property Values in an entity)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static List<Object> ToValues<T>(this T Param)
        {
            List<Object> Values = new List<Object>();
            Param.GetType().GetProperties().ToList().ForEach(t =>
            {
                Values.Add(t.GetValue(Param));
            });
            return Values;
        }

        /// <summary>
        /// 返回指定的枚举描述值
        /// (Returns the specified enumeration description value)
        /// </summary>
        /// <param name="Param"></param>
        /// <returns>枚举描叙</returns>
        public static string ToDescription(this Enum Param)
        {
            return (Param.GetType().GetField(Param.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute).Description;
        }

        /// <summary>
        /// 获取指定字段的Attribute
        /// </summary>
        /// <typeparam name="TSource">指定实体</typeparam>
        /// <typeparam name="TAttribute">特性</typeparam>
        /// <param name="Param">指定实体</param>
        /// <param name="FieldName">字段名</param>
        /// <returns>Attribute</returns>
        public static TAttribute ToAttribute<TSource, TAttribute>(this TSource Param, string FieldName) where TAttribute : Attribute
        {
            return (Param.GetType().GetField(FieldName).GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute);
        }

        #endregion Sync

        #region Async

        /// <summary>
        /// 使用AutoMapper将实体映射到另一个实体并返回该实体(Map an entity to another entity and return the entity by AutoMapper)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<T> ToAutoMapperAsync<T>(this Object obj)
        {
            return await Task.Run(() => ToAutoMapper<T>(obj));
        }

        /// <summary>
        /// 使用AutoMapper将实体映射到另一个实体并返回该实体(Map an entity to another entity and return the entity by AutoMapper)
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToAutoMapperAsync<K,T>(this Object obj)
        {
            return await Task.Run(() => ToAutoMapper<K,T>(obj));
        }

        /// <summary>
        /// 使用AutoMapper将实体映射到另一个实体并返回该实体(Map an entity to another entity and return the entity by AutoMapper)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="IgnoreNames"></param>
        /// <returns></returns>
        public static async Task<T> ToAutoMapperWithIgnoreAsync<T>(this Object obj, String IgnoreNames)
        {
            return await Task.Run(() => ToAutoMapperWithIgnore<T>(obj, IgnoreNames));
        }

        /// <summary>
        ///  返回具有标记为描述属性字段的属性值的实体
        ///  (Returns an entity with a property value marked to describe the property field)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static async Task<String> ToDesAsync<T>(this T Param, Expression<Func<T, object>> Expres)
        {
            return await Task.Run(() => ToDes(Param, Expres));
        }

        /// <summary>
        /// 将实体的属性名称和属性值遍历包含到字典中并返回字典
        /// (Wraps an entity's property name and property value traversal into the dictionary and returns the dictionary)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<Dictionary<String, Object>> ToDicAsync<T>(this T Param)
        {
            return await Task.Run(() => ToDic(Param));
        }

        /// <summary>
        ///  循环字典(Traversing the dictionary)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        public static async Task ToDicEachAsync<T, K>(this IDictionary<T, K> Param, Action<T, K> Selector)
        {
            await Task.Run(() => ToDicEach(Param, Selector));
        }

        /// <summary>
        /// 循环数组(Traversing the array)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Selector"></param>
        public static async Task ToEachAsync<T>(this Array Param, Action<T> Selector)
        {
            await Task.Run(() => ToEach(Param, Selector));
        }

        /// <summary>
        ///  循环集合(Traverse collection)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="Selector"></param>
        public static async Task ToEachsAsync<T>(this IEnumerable<T> queryable, Action<T> Selector)
        {
            await Task.Run(() => ToEachs(queryable, Selector));
        }

        /// <summary>
        /// 将数据表转换为实体(Convert a data table to entities)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToEntitiesAsync<T>(this DataTable Param) where T : new()
        {
            return await Task.Run(() => ToEntities<T>(Param));
        }

        /// <summary>
        /// 将数据表转换为实体(Convert a data table to an entity)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<T> ToEntityAsync<T>(this DataTable Param) where T : new()
        {
            return await Task.Run(() => ToEntity<T>(Param));
        }

        /// <summary>
        /// 序列化(SerializeObject)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ToJsonAsync<T>(this T Param)
        {
            return await Task.Run(() => ToJson(Param));
        }

        /// <summary>
        /// 将实体映射到另一个实体并返回该实体(Map an entity to another entity and return the entity)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<K> ToMapAsync<T, K>(this T Param)
        {
            return await Task.Run(() => ToMapper<T, K>(Param));
        }

        /// <summary>
        ///  将集合映射到另一个集合并返回该集合(Map a collection to another collection and return the collection)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static async Task<List<K>> ToMapsAsync<T, K>(this IEnumerable<T> queryable)
        {
            return await Task.Run(() => ToMappers<T, K>(queryable));
        }

        /// <summary>
        /// 返序列化(DeserializeObject)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<T> ToModelAsync<T>(this String Param)
        {
            return await Task.Run(() => ToModel<T>(Param));
        }

        /// <summary>
        /// 使用MsgPack序列化(DeserializeObject For MessagePack)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="IsPublic"></param>
        /// <returns></returns>
        public static async Task<Byte[]> ToMsgByteAsync<T>(this T Param, Boolean IsPublic = true)
        {
            return await Task.Run(() => ToMsgByte<T>(Param, IsPublic));
        }

        /// <summary>
        /// 使用MsgPack序列化为Json(DeserializeObject To Json For MessagePack)
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ToMsgJsonAsync(this Byte[] Param)
        {
            return await Task.Run(() => ToMsgJson(Param));
        }

        /// <summary>
        /// 使用MsgPack反序列化(DeserializeObject For MessagePack)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="IsPublic"></param>
        /// <returns></returns>
        public static async Task<T> ToMsgModelAsync<T>(this Byte[] Param, Boolean IsPublic = true)
        {
            return await Task.Run(() => ToMsgModel<T>(Param, IsPublic));
        }

        /// <summary>
        ///  返回实体中所有的字段名(Returns all Property names in an entity)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<List<String>> ToNamesAsync<T>(this T Param)
        {
            return await Task.Run(() => ToNames(Param));
        }

        /// <summary>
        /// 返回集合中字段的所有值(Returns all values of a field in a collection)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static async Task<List<Object>> ToOverAsync<T>(this IEnumerable<T> queryable, Expression<Func<T, Object>> Expres)
        {
            return await Task.Run(() => ToOver(queryable, Expres));
        }

        /// <summary>
        ///  返回分页数据(Return paging data)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static async Task<PageResult<T>> ToPageAsync<T>(this IEnumerable<T> queryable, int PageIndex, int PageSize)
        {
            return await Task.Run(() => ToPage(queryable, PageIndex, PageSize));
        }

        /// <summary>
        ///  为选择的T的属性设置一个值(set a value for T's Property which choose)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <param name="Value"></param>
        public static async Task ToSetAsync<T>(this T Param, Expression<Func<T, object>> Expres, Object Value)
        {
            await Task.Run(() => ToSet(Param, Expres, Value));
        }

        /// <summary>
        /// 将实体转换为数据表并返回数据表(Convert the entity to a data table and return the data table)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<DataTable> ToTableAsync<T>(this T Param)
        {
            return await Task.Run(() => ToTable(Param));
        }

        /// <summary>
        ///  返回数据表(return data table)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static async Task<DataTable> ToTableAsync<T>(this IEnumerable<T> queryable, int PageIndex, int PageSize)
        {
            return await Task.Run(() => ToTable(queryable, PageIndex, PageSize));
        }

        /// <summary>
        /// 将集合转换为数据表并返回数据表(Convert the collection to a data table and return the data table)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static async Task<DataTable> ToTablesAsync<T>(this IList<T> queryable)
        {
            return await Task.Run(() => ToTables(queryable));
        }

        /// <summary>
        /// 转换成Unicode(Return Unicode string)
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ToUnicAsync(this String Param)
        {
            return await Task.Run(() => ToUnicode(Param));
        }

        /// <summary>
        /// 替换实体中的数据并将其作为Unicode返回(Replace the data in the entity and return it as Unicode)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<T> ToUnicAsync<T>(this T Param)
        {
            return await Task.Run(() => ToUnicode(Param));
        }

        /// <summary>
        /// 转换成UTF8(Return UTF8 string)
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ToUTF8Async(this String Param)
        {
            return await Task.Run(() => ToUTF8(Param));
        }

        /// <summary>
        ///  替换实体中的数据并将其作为UTF8返回(Replace the data in the entity and return it as UTF8)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<T> ToUTF8Async<T>(this T Param)
        {
            return await Task.Run(() => ToUTF8(Param));
        }

        /// <summary>
        /// 返回实体中所有的字段值(Returns all Property Values in an entity)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<List<Object>> ToValuesAsync<T>(this T Param)
        {
            return await Task.Run(() => ToValues(Param));
        }

        /// <summary>
        /// 返回指定的枚举描述值
        /// (Returns the specified enumeration description value)
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<string> ToDescriptionAsync(this Enum Param)
        {
            return await Task.Run(() => ToDescription(Param));
        }

        /// <summary>
        /// 获取指定字段的Attribute
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="Param"></param>
        /// <param name="FieldName">字段名</param>
        /// <returns></returns>
        public static async Task<TAttribute> ToAttributeAsync<TSource, TAttribute>(this TSource Param, string FieldName) where TAttribute : Attribute
        {
            return await Task.Run(() => ToAttribute<TSource, TAttribute>(Param, FieldName));
        }

        #endregion Async

        #region IsWhat

        /// <summary>
        /// 是否在里面(Is it inside)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="inValues"></param>
        /// <returns></returns>
        public static bool IsContainsIn(this string thisValue, params string[] inValues)
        {
            return inValues.Any(it => thisValue.Contains(it));
        }

        /// <summary>
        /// 是邮箱(Is Email)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsEamil(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
        }

        /// <summary>
        /// 是传真(Is Fax)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsFax(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
        }

        /// <summary>
        /// 是身份证(Is IdCard)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsIDcard(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
        }

        /// <summary>
        /// 是否在里面(Is it inside)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsIn<T>(this T thisValue, params T[] values)
        {
            return values.Contains(thisValue);
        }

        /// <summary>
        /// 值所的范围(Range of values)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsInRange(this int thisValue, int begin, int end)
        {
            return thisValue >= begin && thisValue <= end;
        }

        /// <summary>
        /// 时间值所的范围(Range of datetime values)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsInRange(this DateTime thisValue, DateTime begin, DateTime end)
        {
            return thisValue >= begin && thisValue <= end;
        }

        /// <summary>
        /// Is INT
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsInt(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^\d+$");
        }

        /// <summary>
        /// 是手机(Is Phone)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsMobile(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^\d{11}$");
        }

        /// <summary>
        /// Is Not INT?
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsNoInt(this object thisValue)
        {
            if (thisValue == null) return true;
            return !Regex.IsMatch(thisValue.ToString(), @"^\d+$");
        }

        /// <summary>
        /// 是null或""(Is null or "")
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return true;
            return thisValue.ToString() == "";
        }

        /// <summary>
        /// 是null或""(Is null or "")
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this Guid? thisValue)
        {
            if (thisValue == null) return true;
            return thisValue == Guid.Empty;
        }

        /// <summary>
        ///  确定集合是否为空(Determine if the collection is empty)
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this IEnumerable<object> queryable)
        {
            return queryable == null || !queryable.Any();
        }

        /// <summary>
        /// 是座机(Is Tel Phone)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsTelephone(this object thisValue)
        {
            if (thisValue == null) return false;
            return Regex.IsMatch(thisValue.ToString(), @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{8}$");
        }

        /// <summary>
        /// 有值?(has value)
        /// </summary>
        /// <returns></returns>
        public static bool IsValuable(this object thisValue)
        {
            if (thisValue == null) return false;
            return thisValue.ToString() != "";
        }

        /// <summary>
        /// 有值?(has value)
        /// </summary>
        /// <returns></returns>
        public static bool IsValuable(this IEnumerable<object> thisValue)
        {
            if (thisValue == null || thisValue.Count() == 0) return false;
            return true;
        }

        /// <summary>
        /// 是零(IsZero)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsZero(this object thisValue)
        {
            return (thisValue == null || thisValue.ToString() == "0");
        }

        /// <summary>
        /// 是否字母或数字
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool IsNumOrLetter(this string thisValue)
        {
            return Regex.IsMatch(thisValue, "^[a-zA-Z\\d]+$");
        }
        #endregion IsWhat

        #region Encryption

        #region Sync

        /// <summary>
        /// LzString解密（LzString Base64 Decryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static String ToLzStringDec(this String Param)
        {
            return LzStringEncryption.DecompressFromBase64(Param);
        }

        /// <summary>
        /// LzString加密（LzString Base64 Encryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static String ToLzStringEnc(this String Param)
        {
            return LzStringEncryption.CompressToBase64(Param);
        }
        /// <summary>
        /// MD5加密（MD5 Encryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="type">位数：16 32</param>
        /// <returns></returns>
        public static String ToMD5(this String Param, int type = 32)
        {
            if (type != 32 && type != 16)
                return "Please enter the MD5 encryption digits,for example：16、32";
            return type == 32 ? MD5Encryption.MD5_32(Param) : MD5Encryption.MD5_16(Param);
        }

        /// <summary>
        /// RSA解密（RSA Decryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
        public static String ToRSADec(this String Param,String PrivateKey)
        {
            return RSAEncryption.RSADecrypt(Param, PrivateKey);
        }

        /// <summary>
        /// RSA加密（RSA Encryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="PublicKey"></param>
        /// <returns></returns>
        public static String ToRSAEnc(this String Param,String PublicKey)
        {
            return RSAEncryption.RSAEncrypt(Param,PublicKey);
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
        public static String ToSign(this String Param, String PrivateKey) 
        {
            return RSAEncryption.Sign(Param, PrivateKey);
        }

        /// <summary>
        /// SHA加密（SHA Encryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="type">位数：1 256 384 512</param>
        /// <returns></returns>
        public static String ToSHA(this String Param, int type = 1)
        {
            if (type != 1 && type != 256 && type != 384 && type != 512)
                return "Please enter the number of SHA encryption bits, for example：1、256、384、512";
            switch (type)
            {
                case 1:
                    return SHAEncryption.SHA1Encrypt(Param);

                case 256:
                    return SHAEncryption.SHA256Encrypt(Param);

                case 384:
                    return SHAEncryption.SHA384Encrypt(Param);

                case 512:
                    return SHAEncryption.SHA512Encrypt(Param);

                default:
                    return SHAEncryption.SHA1Encrypt(Param);
            }
        }
        #endregion Sync

        #region Async

        /// <summary>
        /// LzString解密（LzString Base64 Decryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ToLzStringAsyncDec(this String Param)
        {
            return await Task.Run(() => ToLzStringDec(Param));
        }

        /// <summary>
        /// LzString加密（LzString Base64 Encryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static async Task<String> ToLzStringAsyncEnc(this String Param)
        {
            return await Task.Run(() => ToLzStringEnc(Param));
        }
        /// <summary>
        /// MD5加密（MD5 Encryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="type">位数：16 32</param>
        /// <returns></returns>
        public static async Task<String> ToMD5Async(this String Param, int type = 32)
        {
            return await Task.Run(() => ToMD5(Param, type));
        }

        /// <summary>
        /// RSA解密（RSA Decryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
        public static async Task<String> ToRSAAsyncDec(this String Param,String PrivateKey)
        {
            return await Task.Run(() => ToRSADec(Param, PrivateKey));
        }

        /// <summary>
        /// RSA加密（RSA Encryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="PublicKey"></param>
        /// <returns></returns>
        public static async Task<String> ToRSAAsyncEnc(this String Param, String PublicKey)
        {
            return await Task.Run(() => ToRSAEnc(Param, PublicKey));
        }

        /// <summary>
        /// SHA加密（SHA Encryption）
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="type">位数：1 256 384 512</param>
        /// <returns></returns>
        public static async Task<String> ToSHAAsync(this String Param, int type = 1)
        {
            return await Task.Run(() => ToSHA(Param, type));
        }
        #endregion Async

        #endregion Encryption
    }
}