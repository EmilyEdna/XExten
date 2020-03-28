using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxyServer.MiddleView.ViewInterface
{
    public interface ISocketSession
    {
        /// <summary>
        /// 用户主键
        /// </summary>
        string PrimaryKey { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        string SessionAccount { get; set; }
        /// <summary>
        /// 用户权限
        /// </summary>
        string SessionRole { get; set; }
        /// <summary>
        /// 自定义数据
        /// </summary>
        object CustomizeData { get; set; }
    }
}
