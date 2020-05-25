using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Tracing.Entry.Enum;
using XExten.XCore;

namespace XExten.Profile.Tracing
{
    public class MethondDiagnosticHandler : IMethondDiagnosticHandler
    {

        public void Handle(ITracingContext tracingContext, Object Provider)
        {
            if (Provider is Dictionary<string, object> ProviderData)
            {
                var MethodName = ProviderData["MethodName"].ToString();
                ProviderData.Remove("MethodName");
                var Parameters = ProviderData.ToJson();
                var Partial = tracingContext.CreateLocalPartialContext(MethodName);
                Partial.Context.Component = "METHODINVOKE";
                Partial.Context.LayerType = ChannelLayerType.Method;
                Partial.Context.Add("Method", MethodName);
                Partial.Context.Add("Parameters", Parameters);
            }
        }

        public bool OnlyMatch(Object Data) => true;
    }
}
