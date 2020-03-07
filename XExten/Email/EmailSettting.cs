using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Email
{
    /// <summary>
    /// 邮箱配置
    /// </summary>
    public class EmailSettting
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public static string SmtpHost { get; set; }
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public static string EmailAccount { get; set; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public static string EmailPassWord { get; set; }
        /// <summary>
        /// 发送标题
        /// </summary>
        public static string SendTitle { get; set; }
    }
}
