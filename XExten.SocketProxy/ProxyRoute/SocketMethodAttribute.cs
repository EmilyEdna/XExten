using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxy.SocketRoute
{
    /// <summary>
    /// Socket方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SocketMethodAttribute : Attribute
    {
        /// <summary>
        /// 自定义方法名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 自定义方法版本
        /// </summary>
        public string MethodVersion { get; set; }
    }
}
