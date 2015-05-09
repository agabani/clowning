using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace clowning.communicationsprotocol
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

        public async Task Send(string message, CancellationToken cancellationToken)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await _networkStream.WriteAsync(buffer, 0, buffer.Count(), cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _networkStream.Dispose();
            _tcpClient.Close();
        }
    }
}