using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace XExten.Profile.AspNetCore.Source
{
    public class SocketHandlerDiagnosticListener
    {
        private static readonly DiagnosticSource DiagnosticListener;

        static SocketHandlerDiagnosticListener()
        {
            DiagnosticListener = new DiagnosticListener("MethodHandlerDiagnosticListener");
        }
        public static SocketHandlerDiagnosticListener SocketListener => new SocketHandlerDiagnosticListener();

        public void ExcuteSocketBeginReceive(Object Param)
        {
            if (DiagnosticListener.IsEnabled("ExcuteSocketBeginReceive"))
                DiagnosticListener.Write("ExcuteSocketBeginReceive", Param);
        }
        public void ExcuteSocketBeginEnd(Object Param)
        {
            if (DiagnosticListener.IsEnabled("ExcuteSocketBeginEnd"))
                DiagnosticListener.Write("ExcuteSocketBeginEnd", Param);
        }
        public void ExcuteSocketException(Exception exception)
        {
            if (DiagnosticListener.IsEnabled("ExcuteSocketException"))
                DiagnosticListener.Write("ExcuteSocketException", exception);
        }
    }
}
