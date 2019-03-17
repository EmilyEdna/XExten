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
        ///  Get Attr
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="Express"></param>
        /// <returns></returns>
        T GetAttributeType<T, K>(Expression<Func<K, Object>> Express);
        /// <summary>
        /// Set T's ProertyValues
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonValue"></param>
        /// <param name="t"></param>
        void SetProptertiesValue<T>(Dictionary<String,Object> JsonValue, T Param) where T : class, new();
        /// <summary>
        /// Create anonmouse type expression ex：(t=> new {x1,x2})
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="AnonmouseType"></param>
        /// <returns></returns>
        Expression<Func<T, Object>> GetExpression<T>(Type AnonmouseType) where T : class, new();
    }
}
