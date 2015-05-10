using clowning.communicationsprotocol.Stream;

namespace clowning.communicationsprotocol.Json.Stream
{
    public class JsonPacketStreamFactory : IPacketStreamFactory
    {
        public IPacketStream New()
        {
            return new JsonPacketStream();
        }
    }
}