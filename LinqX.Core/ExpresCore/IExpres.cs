using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LinqX.Core.ExpresCore
{
    public interface IExpres
    {
        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Express"></param>
        /// <returns></returns>
        T GetAttributeType<T, K>(Expression<Func<K, Object>> Express);
        /// <summary>
        /// 设置属性的名称和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonValue"></param>
        /// <param name="t"></param>
        void SetProptertiesValue<T>(Dictionary<String,Object> JsonValue, T Param) where T : class, new();
        /// <summary>
        /// 生成表达式 ex：(t=> new {t1,t2})
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="AnonmouseType"></param>
        /// <returns></returns>
        Expression<Func<T, Object>> GetExpression<T>(Type AnonmouseType) where T : class, new();
    }
}
