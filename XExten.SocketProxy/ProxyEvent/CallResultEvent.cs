using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxy.SocketEvent
{
    public class CallResultEvent: EventArgs
    {
        private Dictionary<String, Object> Result;
        public CallResultEvent(Dictionary<String, Object> Param)
        {
            Result = Param;
        }
        /// <summary>
        /// 结果检查
        /// </summary>
        internal bool ResultCheck
        {
            get
            {
                if (Result == null) return false;
                if (Result.Count == 0) return false;
                return true;
            }
        }
    }
}
