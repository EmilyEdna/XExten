using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxy.SocketConfig
{
    public class SocketSerializeData
    {
        public String Route { get; set; }
        public Dictionary<String, Object> Providor { get; set; }
        public SocketSerializeData AppendSerialized(string Key, object Value)
        {
            Providor ??= new Dictionary<String, Object>();
            Providor.Add(Key, Value);
            return this;
        }
        public SocketSerializeData AppendRoute(string Router)
        {
            Route = Router.ToLower();
            return this;
        }
    }
}
