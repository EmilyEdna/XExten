using BeetleX.Buffers;
using BeetleX.Clients;
using BeetleX.Packets;
using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.SocketProxy.SocketConfig
{
    public class SocketPacket : FixeHeaderClientPacket
    {
        public IMessageTypeHeader TypeHeader { get; set; }

        public SocketPacket()
        {
            TypeHeader = new TypeHandler();
        }

        public override IClientPacket Clone()
        {
            return new SocketPacket();
        }

        protected override object OnRead(IClient client, PipeStream stream)
        {
            return MessagePackSerializer.Deserialize(TypeHeader.ReadType(stream), stream, MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance));
        }

        protected override void OnWrite(object data, IClient client, PipeStream stream)
        {
            TypeHeader.WriteType(data, stream);
            MessagePackSerializer.Serialize(data.GetType(), stream, data, MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance));
        }
    }
}
