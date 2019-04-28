using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using XExten.MessageQueue;
using Xunit;

namespace XExten.Test
{
    public class QueueTestClass
    {
        [Fact]
        public void QueueTest()
        {
            XQueue mq = new XQueue();
            var count = mq.GetCount();
            var key = Guid.NewGuid().ToString();
            //Test Add()
            var item = mq.Add(key, () => Trace.WriteLine("测试写入"));
            Assert.Equal(count + 1, mq.GetCount());
            //Test GetCurrentKey()
            var currentKey = mq.GetCurrentKey();
            Assert.Equal(key, currentKey);
            //Test GetItem
            var currentItem = mq.GetItem(currentKey);
            Assert.Equal(currentItem.Key, item.Key);
            Assert.Equal(currentItem.AddTime, item.AddTime);
            //Test Remove
            mq.Remove(key);
            Assert.Equal(count, mq.GetCount());
            // Test Exec
            XQueue.OperateQueue();
        }
    }
}
