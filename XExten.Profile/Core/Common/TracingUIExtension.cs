using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XExten.Profile.Tracing.Entry;
using XExten.XCore;

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
            WebClient Client = new WebClient();
            Client.Headers.Add("Content-Type", "application/json");
            Client.UploadData(UIHost, "POST", Encoding.UTF8.GetBytes(Context.ToJson()));
        }
    }
}
