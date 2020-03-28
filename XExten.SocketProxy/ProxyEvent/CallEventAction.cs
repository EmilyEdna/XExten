using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace XExten.SocketProxy.SocketEvent
{
    public class CallEventAction
    {
        private Dictionary<String, Object> ResponseResult;
        private static readonly Dictionary<String, CallEventAction> Cache = new Dictionary<string, CallEventAction>();
        /// <summary>
        /// 实例
        /// </summary>
        /// <returns></returns>
        public static CallEventAction Instance()
        {
            if (Cache.ContainsKey(typeof(CallEventAction).Name))
            {
                var Instance = Cache.Values.FirstOrDefault();
                Instance.ResponseResult = null;
                return Instance;
            }
            else
            {
                var Instance = new CallEventAction();
                Cache.Add(Instance.GetType().Name, Instance);
                return Instance;
            }
        }
        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Event"></param>
        public void OnResponse(Object Sender, EventArgs Event)
        {
            if ((Event as CallResultEvent).ResultCheck)
                ResponseResult = (Sender as CallEvent).Response;
            else
                ResponseResult = null;
        }
        /// <summary>
        /// 事件结果
        /// </summary>
        public Dictionary<String, Object> DelegateResult => IsNull();
        /// <summary>
        /// 处理空结果
        /// </summary>
        /// <returns></returns>
        protected Dictionary<String, Object> IsNull()
        {
            if (ResponseResult == null)
            {
                Thread.Sleep(100);
                return IsNull();
            }
            else
                return ResponseResult;
        }
    }
}
