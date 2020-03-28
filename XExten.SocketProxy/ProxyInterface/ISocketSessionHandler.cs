using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxy.SocketInterface
{
    public interface ISocketSessionHandler
    {
        bool Executing(ISocketSession Session);
    }
}
