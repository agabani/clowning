using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace clowning.communicationsprotocol.Models
{
    public class TcpClientContext : IDisposable
    {
        public int Id { get; private set; }

        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _networkStream;

        public TcpClientContext(int id, TcpClient tcpClient)
        {
            Id = id;
            _tcpClient = tcpClient;
            _networkStream = tcpClient.GetStream();
        }

        public async Task Send(byte[] message, CancellationToken cancellationToken)
        {
            await _networkStream.WriteAsync(message, 0, message.Count(), cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _networkStream.Dispose();
            _tcpClient.Close();
        }
    }
}