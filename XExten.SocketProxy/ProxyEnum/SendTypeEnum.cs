using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XExten.SocketProxy.SocketEnum
{
    /// <summary>
    /// Socket发送状态
    /// </summary>
    public enum SendTypeEnum
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [Description("初始化")]
        Init=0,
        /// <summary>
        /// 内部通信
        /// </summary>
        [Description("内部通信")]
        InternalInfo =1,
        /// <summary>
        /// 请求通信
        /// </summary>
        [Description("请求通信")]
        RequestInfo =2,
        /// <summary>
        /// 回调通信
        /// </summary>
        [Description("回调通信")]
        CallBack =3
    }
}
