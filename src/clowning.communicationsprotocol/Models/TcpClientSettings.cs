using clowning.communicationsprotocol.Stream;

namespace clowning.communicationsprotocol.Models
{
    public class TcpClientSettings
    {
        public int Port { get; set; }
        public string Hostname { get; set; }
        public IPacketStreamFactory PacketStreamFactory { get; set; }
        public int ConnectionTimeoutPeriod { get; set; }
    }
}