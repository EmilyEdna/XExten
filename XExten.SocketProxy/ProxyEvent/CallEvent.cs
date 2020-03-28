using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XExten.SocketProxy.SocketEvent
{
    public class CallEvent
    {
        #region Event
        /// <summary>
        /// 事件
        /// </summary>
        public event ResultEventHandler Changed;
        /// <summary>
        /// 委托事件
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Args"></param>
        public delegate void ResultEventHandler(object Sender, EventArgs Args);
        #endregion

        #region Property
        /// <summary>
        /// 保证是同一个对象
        /// </summary>
        private static readonly Dictionary<String, CallEvent> Cache = new Dictionary<String, CallEvent>();
        private Dictionary<String, Object> Result;
        /// <summary>
        /// 结果
        /// </summary>
        public Dictionary<String, Object> Response
        {
            get { return Result; }
            set { Result = value; OnChanged(new CallResultEvent(value)); }
        }
        #endregion

        #region Instance
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        public static CallEvent Instance()
        {
            if (Cache.ContainsKey(typeof(CallEvent).Name)) return Cache.Values.FirstOrDefault();
            else
            {
                var Instance = new CallEvent();
                Cache.Add(Instance.GetType().Name, Instance);
                return Instance;
            }
        }
        #endregion

        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="Args"></param>
        protected virtual void OnChanged(EventArgs Args)
        {
            Changed?.Invoke(this, Args);
        }
    }
}
