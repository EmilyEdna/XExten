using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiInterface;

namespace XExten.HttpFactory.MultiImplement
{
    /// <summary>
    /// Cookie
    /// </summary>
    public class Cookies : ICookies
    {
        internal CookieContainer Containers;

        /// <summary>
        /// Constructor
        /// </summary>
        public Cookies()
        {
            Containers = new CookieContainer();
        }
        /// <summary>
        /// Constructor with CookieContainer
        /// </summary>
        /// <param name="Container"></param>
        public Cookies(CookieContainer Container)
        {
            Containers = Container;
        }

        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICookies Cookie(string name, string value)
        {
            Cookie Cookie = new Cookie(name, value);
            Containers.Add(Cookie);
            return this;
        }
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ICookies Cookie(string name, string value, string path)
        {
            Cookie Cookie = new Cookie(name, value,path);
            Containers.Add(Cookie);
            return this;
        }
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public ICookies Cookie(string name, string value, string path, string domain)
        {
            Cookie Cookie = new Cookie(name, value,path,domain);
            Containers.Add(Cookie);
            return this;
        }
    }
}
