using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.HttpFactory.MultiInterface;

namespace XExten.HttpFactory.MultiImplement
{
    public class Node : INode
    {
        public INode AddUrl(string Path)
        {
            throw new NotImplementedException();
        }

        public ICookies Cookie(string name, string value)
        {
            throw new NotImplementedException();
        }

        public ICookies Cookie(string name, string value, string path)
        {
            throw new NotImplementedException();
        }

        public ICookies Cookie(string name, string value, string path, string domain)
        {
            throw new NotImplementedException();
        }

        public IHeaders Header(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IHeaders Header(Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
        }
    }
}
