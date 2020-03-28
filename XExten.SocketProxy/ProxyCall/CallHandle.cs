using XExten.SocketProxy.SocketConfig;
using XExten.SocketProxy.SocketDependency;
using XExten.SocketProxy.SocketInterface;
using XExten.SocketProxy.SocketRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XExten.XCore;
using System.Text;
using XExten.Common;
using System.Threading.Tasks;

using XExten.SocketProxy.ProxyAbstract;

namespace XExten.SocketProxy.SocketCall
{
    public class CallHandle: CallHandleAbstract
    {
        /// <summary>
        /// 逆向解析方法
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public override ISocketResult ExecuteCallFuncHandler(SocketMiddleData Param)
        {
          return base.ExecuteCallFuncHandler(Param);
        }
        /// <summary>
        /// 处理Session
        /// </summary>
        /// <param name="Controller"></param>
        /// <param name="Method"></param>
        protected override bool ExecuteCallSessionHandler(MethodInfo Method, ISocketSession Session)
        {
            return base.ExecuteCallSessionHandler(Method, Session);
        }
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="Controller"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        protected override ISocketResult ExecuteCallDataHandler(Type Controller, MethodInfo Method, ISocketResult Param)
        {
            return base.ExecuteCallDataHandler(Controller, Method, Param);
        }
    }
}
