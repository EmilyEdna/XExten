using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.HttpFactory.MultiInterface
{
    /// <summary>
    /// Header
    /// </summary>
    public interface IHeaders
    {
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
        /// <returns></returns>
        ICookies Cookie();
        /// <summary>
        /// Add Path
        /// </summary>
        /// <returns></returns>
        INode Node();
    }
}
