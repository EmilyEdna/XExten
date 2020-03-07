using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Email
{
    /// <summary>
    /// 邮箱内容
    /// </summary>
   public class EmailViewModel
    {
        /// <summary>
        /// 收件人邮箱
        /// </summary>
        public string AcceptedAddress { get; set; }
        /// <summary>
        /// 发送标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
