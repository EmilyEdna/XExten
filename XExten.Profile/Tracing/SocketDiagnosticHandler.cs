using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Tracing.Entry.Enum;

namespace XExten.Profile.Tracing
{
    public class SocketDiagnosticHandler : ISocketDiagnosticHandler
    {
        public void Handle(ITracingContext tracingContext, Object Provider)
        {
            if (Provider is Dictionary<string, object> ProviderData)
            {
                var Ways = $"{ProviderData["AddressWays"]}_{ProviderData["Ways"]}";
                var Header = new List<KeyValuePair<string, string>> {
                       new KeyValuePair<string, string>( "Ways", ProviderData["AddressWays"].ToString()),
                       new KeyValuePair<string, string>("Type",  ProviderData["Ways"].ToString())
                };
                var Partial = tracingContext.CreateEntryPartialContext(Ways, new CarrierHeaderCollection(Header));
                Partial.Context.Component = "Socket";
                Partial.Context.LayerType = ChannelLayerType.Socket;
                Partial.Context.Router = ProviderData["Remote"].ToString();
                Partial.Context.Method = $"{ProviderData["AddressWays"]}_{ProviderData["Ways"]}";
                Partial.Context.Add("LocalPoint", ProviderData["Local"].ToString());
                Partial.Context.Add("SocketType", ProviderData["SocketType"].ToString());
            }
        }

        public bool OnlyMatch(object Data)
        {
            return true;
        }
    }
}
