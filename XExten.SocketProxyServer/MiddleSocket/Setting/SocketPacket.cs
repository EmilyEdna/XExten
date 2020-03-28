using BeetleX;
using BeetleX.Buffers;
using BeetleX.Packets;
using MessagePack;
using MessagePack.Resolvers;

namespace XExten.SocketProxyServer.MiddleSocket.Setting
{
    public class SocketPacket: FixedHeaderPacket
    {
        public IMessageTypeHeader TypeHeader { get; set; }
        public SocketPacket()
        {
            TypeHeader = new TypeHandler();
        }
        public override IPacket Clone()
        {
            return new SocketPacket();
        }
        protected override object OnRead(ISession session, PipeStream stream)
        {
            return MessagePackSerializer.Deserialize(TypeHeader.ReadType(stream), stream, MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance));
        }

        protected override void OnWrite(ISession session, object data, PipeStream stream)
        {
            TypeHeader.WriteType(data, stream);
            MessagePackSerializer.Serialize(data.GetType(), stream, data, MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance));
        }
    }
}
