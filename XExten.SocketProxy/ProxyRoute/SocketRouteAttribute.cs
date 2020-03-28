using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxy.SocketRoute
{
    /// <summary>
    /// Socket路由
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SocketRouteAttribute : Attribute
    {
        /// <summary>
        /// 内部服务名称
        /// </summary>
        public string InternalServer { get; set; }
        /// <summary>
        /// 自定义控制器名称
        /// </summary>
        public string ControllerName { get; set; }
        public SocketRouteAttribute(string Server)
        {
            InternalServer = Server;
        }
    }
}
