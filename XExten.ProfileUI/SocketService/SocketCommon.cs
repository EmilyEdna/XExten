using BeetleX;
using BeetleX.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.ProfileUI.ConfigHelp;
using XExten.ProfileUI.DbFactory;
using XExten.ProfileUI.Model.EFModel;

namespace XExten.ProfileUI.SocketService
{
    public class SocketCommon : ServerHandlerBase
    {
        public static void InitSocket()
        {
            IServer Server = SocketFactory.CreateTcpServer<SocketCommon>();
            Server.Options.DefaultListen.Port = Convert.ToInt32(ConfigReader.GetSecetion("SocketPort")); ;
            Server.Options.DefaultListen.Host = ConfigReader.GetSecetion("SocketHost");
            Server.Open();
        }
        public override void SessionReceive(IServer Server, SessionReceiveEventArgs Event)
        {
            var Pipe = Event.Stream.ToPipeStream();
            if (Pipe.TryReadLine(out string Data))
            {
                using MemoryDb db = new MemoryDb();
                var View = db.Traces.Add(new TraceModel
                {
                    Result = Data,
                    CreateTime = DateTime.Now
                });
                db.SaveChanges();
            }
        }
    }
}
