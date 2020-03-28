using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxy.SocketInterface.DefaultImpl
{
    public class SocketSessionDefault : ISocketSession
    {
        public string PrimaryKey { get; set; }
        public string SessionAccount { get; set; }
        public string SessionRole { get; set; }
        public object CustomizeData { get; set; }
        public static SocketSessionDefault SetValue(string PrimaryKey, string SessionAccount, string SessionRole, object CustomizeData)
        {
            return new SocketSessionDefault
            {
                PrimaryKey = PrimaryKey,
                SessionAccount = SessionAccount,
                SessionRole = SessionRole,
                CustomizeData = CustomizeData
            };
        }
    }
}
