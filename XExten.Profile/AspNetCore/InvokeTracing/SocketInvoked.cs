using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using XExten.Profile.AspNetCore.Source;
using XExten.XPlus;

namespace XExten.Profile.AspNetCore.InvokeTracing
{
    public static class SocketInvoked
    {
        public static void ByTraceSocket(this Socket Socket,Object? data)
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("Ways", Socket.ProtocolType.ToString());
            keyValues.Add("Remote", Socket.RemoteEndPoint.ToString());
            keyValues.Add("Local", Socket.LocalEndPoint.ToString());
            keyValues.Add("AddressWays", Socket.AddressFamily.ToString());
            keyValues.Add("SocketType", Socket.SocketType.ToString());
            SocketHandlerDiagnosticListener.SocketListener.ExcuteSocketBeginReceive(keyValues);
            XPlusEx.XTry(() =>
            {
                SocketHandlerDiagnosticListener.SocketListener.ExcuteSocketBeginEnd(keyValues);
            }, ex => SocketHandlerDiagnosticListener.SocketListener.ExcuteSocketException(ex));
        }
    }
}
