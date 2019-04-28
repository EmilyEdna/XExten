using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.MessageQueue
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueItem
    {
        /// <summary>
        /// 队列项唯一标识
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 队列项目命中触发时执行的委托
        /// </summary>
        public Action Action { get; set; }
        /// <summary>
        /// 此实例对象的创建时间
        /// </summary>
        public DateTimeOffset AddTime { get; set; }
        /// <summary>
        /// 项目说明（主要用于调试）
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="description"></param>
        public QueueItem(string key, Action action,string description=null)
        {
            Key = key;
            Action = action;
            Description = description;
            AddTime = DateTimeOffset.Now;
        }
    }
}
