using BeetleX;
using BeetleX.EventArgs;
using XExten.XCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.ProfileUI.SocketService
{
    public class SocketCommon : ServerHandlerBase
    {
        public static void InitSocket()
        {
            IServer Server = SocketFactory.CreateTcpServer<SocketCommon>();
            Server.Options.DefaultListen.Port = 9374;
            Server.Options.DefaultListen.Host = "127.0.0.1";
            Server.Open();
        }
        public override void SessionReceive(IServer Server, SessionReceiveEventArgs Event)
        {
            var Pipe = Event.Stream.ToPipeStream();
            if (Pipe.TryReadLine(out string Data))
            {
                var Result =  Data.ToModel<JObject>();
            }
        }
    }
}
