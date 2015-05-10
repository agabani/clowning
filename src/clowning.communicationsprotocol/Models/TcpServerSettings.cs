using clowning.communicationsprotocol.Stream;

namespace clowning.communicationsprotocol.Models
{
    public class TcpServerSettings
    {
        public int Port { get; set; }
        public int ConnectionTimeoutPeriod { get; set; }
        public IPacketStreamFactory PacketStreamFactory { get; set; }
    }
}