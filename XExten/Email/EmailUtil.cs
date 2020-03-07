using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using XExten.XCore;
using System.Threading.Tasks;

namespace XExten.Email
{
    /// <summary>
    /// 邮箱工具类
    /// </summary>
    public class EmailUtil
    {
        /// <summary>
        ///  发送邮件
        /// </summary>
        /// <param name="View"></param>
        /// <returns></returns>
        internal static void SendMail(EmailViewModel View)
        {
            if (EmailSettting.EmailAccount.IsNullOrEmpty() || EmailSettting.EmailAccount.IsNullOrEmpty() ||
                EmailSettting.EmailPassWord.IsNullOrEmpty() || EmailSettting.SendTitle.IsNullOrEmpty())
                throw new Exception($"Please check mail setting,There has some  error in the setting at ：{typeof(EmailSettting).FullName}");
            SmtpClient Client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = EmailSettting.EmailAccount,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(EmailSettting.EmailAccount, EmailSettting.EmailPassWord)
            };
            MailAddress From = new MailAddress(EmailSettting.EmailAccount, EmailSettting.SendTitle);
            MailAddress To = new MailAddress(View.AcceptedAddress);
            MailMessage Msg = new MailMessage(From, To)
            {
                Subject = View.Title,
                Body = View.Content,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };
            Client.Send(Msg);
        }
    }
}
