using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.Client.Sender
{
    public interface IHttpTraceSenderProvider
    {
        IHttpTraceSender GetSender();
    }
}
