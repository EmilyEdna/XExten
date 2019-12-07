using System;
using System.Collections.Generic;

namespace XExten.MessageQueue
{
    /// <summary>
    ///
    /// </summary>
    public class QueueDictionary : Dictionary<String, QueueItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public QueueDictionary() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}