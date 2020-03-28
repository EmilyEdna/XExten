using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxy.SocketRoute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SocketAuthorAttribute : Attribute
    {
        public bool UseAuthor { get; set; }
        public SocketAuthorAttribute(bool _Author)
        {
            UseAuthor = _Author;
        }
    }
}
