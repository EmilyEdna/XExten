using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.MessageQueue
{
    public class QueueDictionary : Dictionary<String, QueueItem>
    {
        public QueueDictionary() : base(StringComparer.OrdinalIgnoreCase){ }
    }
}
