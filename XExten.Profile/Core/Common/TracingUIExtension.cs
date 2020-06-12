using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Tracing.Entry;
using XExten.XCore;
using XExten.HttpFactory;

namespace XExten.Profile.Core.Common
{
    internal static class TracingUIExtension
    {
        /// <summary>
        /// UI界面地址
        /// </summary>
        internal static string UIHost { get; set; }
        /// <summary>
        /// 将数据绘制到UI界面
        /// </summary>
        /// <param name="Context"></param>
        internal static void OpenUI(this PartialContext Context)
        {
            var xx = HttpMultiClient.HttpMulti.AddNode("https://www.baidu.com").Build().RunBytes();

            var data = HttpMultiClient.HttpMulti.AddNode(UIHost, Context.ToJson(), RequestType.POST).Build().RunBytes();
        }
    }
}
