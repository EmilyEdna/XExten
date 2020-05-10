using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace XExten.Profile.AspNetCore.MethodSource
{
    public class MethodHandlerDiagnosticListener
    {
        private static readonly DiagnosticSource DiagnosticListener;
        static MethodHandlerDiagnosticListener()
        {
            DiagnosticListener = new DiagnosticListener("MethodHandlerDiagnosticListener");
        }
        public static MethodHandlerDiagnosticListener MethodListener => new MethodHandlerDiagnosticListener();
        public void ExecuteCommandMethodStar(Object Param)
        {
            if (DiagnosticListener.IsEnabled("ExecuteCommandMethodStar"))
                DiagnosticListener.Write("ExecuteCommandMethodStar", Param);
        }
        public void ExecuteCommandMethodEnd(Object Param) {
            if (DiagnosticListener.IsEnabled("ExecuteCommandMethodEnd"))
                DiagnosticListener.Write("ExecuteCommandMethodEnd", Param);
        }
        public void ExecuteCommandMethodException(Exception exception) {
            if (DiagnosticListener.IsEnabled("ExecuteCommandMethodException"))
                DiagnosticListener.Write("ExecuteCommandMethodException", exception);
        }
    }
}
