using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.HttpFactory.MultiInterface
{
    /// <summary>
    /// 
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Add URL
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        INode AddUrl(string Path);
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IHeaders Header(string key, string value);
        /// <summary>
        /// Add Header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        IHeaders Header(Dictionary<string, string> headers);
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ICookies Cookie(string name, string value);
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        ICookies Cookie(string name, string value, string path);
        /// <summary>
        /// Add Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        ICookies Cookie(string name, string value, string path, string domain);
    }
}
