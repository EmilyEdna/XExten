using System;
using System.Linq;

namespace XExten.MessageQueue
{
    /// <summary>
    ///
    /// </summary>
    public class XQueue
    {
        /// <summary>
        /// 队列数据集合
        /// </summary>
        public static QueueDictionary MessageQueueDictionary = new QueueDictionary();

        /// <summary>
        /// 同步执行锁
        /// </summary>
        private static object MessageQueueSyncLock = new object();

        /// <summary>
        /// 立即同步所有缓存执行锁（给OperateQueue()使用）
        /// </summary>
        private static object FlushCacheLock = new object();

        /// <summary>
        /// 生成Key
        /// </summary>
        /// <param name="name">队列应用名称，如“ContainerBag”</param>
        /// <param name="senderType">操作对象类型</param>
        /// <param name="identityKey">对象唯一标识Key</param>
        /// <param name="actionName">操作名称，如“UpdateContainerBag”</param>
        /// <returns></returns>
        public static string GenerateKey(string name, Type senderType, string identityKey, string actionName)
        {
            return string.Format("Name@{0}||Type@{1}||Key@{2}||ActionName@{3}", name, senderType, identityKey, actionName);
        }

        /// <summary>
        /// 操作队列
        /// </summary>
        public static void OperateQueue()
        {
            lock (FlushCacheLock)
            {
                var mq = new XQueue();
                var key = mq.GetCurrentKey(); //获取最新的Key
                while (!string.IsNullOrEmpty(key))
                {
                    var mqItem = mq.GetItem(key); //获取任务项
                    mqItem.Action(); //执行
                    mq.Remove(key); //清除
                    key = mq.GetCurrentKey(); //获取最新的Key
                }
            }
        }

        /// <summary>
        /// 获取当前等待执行的Key
        /// </summary>
        /// <returns></returns>
        public string GetCurrentKey()
        {
            lock (MessageQueueSyncLock)
            {
                return MessageQueueDictionary.Keys.FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取MessageQueueItem
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public QueueItem GetItem(string key)
        {
            lock (MessageQueueSyncLock)
            {
                if (MessageQueueDictionary.ContainsKey(key))
                {
                    return MessageQueueDictionary[key];
                }
                return null;
            }
        }

        /// <summary>
        /// 添加队列成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public QueueItem Add(string key, Action action, string description = null)
        {
            lock (MessageQueueSyncLock)
            {
                var mqItem = new QueueItem(key, action, description);
                MessageQueueDictionary[key] = mqItem;
                return mqItem;
            }
        }

        /// <summary>
        /// 移除队列成员
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            lock (MessageQueueSyncLock)
            {
                if (MessageQueueDictionary.ContainsKey(key))
                {
                    MessageQueueDictionary.Remove(key);
                }
            }
        }

        /// <summary>
        /// 获得当前队列数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            lock (MessageQueueSyncLock)
            {
                return MessageQueueDictionary.Count;
            }
        }
    }
}