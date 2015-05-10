using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using clowning.communicationsprotocol.Models;
using clowning.communicationsprotocol.Stream;

namespace clowning.communicationsprotocol
{
    public class BlyncTcpClient : IDisposable
    {
        private readonly int _port;
        private readonly string _hostname;
        private CancellationTokenSource _cancellationTokenSource;
        private TcpClient _tcpClient;
        private readonly IPacketStreamFactory _packetStreamFactory;
        private readonly int _connectionTimeoutPeriod;
        private NetworkStream _networkStream;

        public event OnConnectedEvent ClientConnectedEvent;
        public event OnDisconnectedEvent ClientDisconnectedEvent;
        public event OnMessageReceivedEvent MessageReceivedEvent;

        public delegate void OnConnectedEvent(object sender, EventArgs args);

        public delegate void OnDisconnectedEvent(object sender, EventArgs args);

        public delegate void OnMessageReceivedEvent(object sender, byte[] args);

        public BlyncTcpClient(TcpClientSettings tcpClientSettings)
        {
            _port = tcpClientSettings.Port;
            _hostname = tcpClientSettings.Hostname;
            _connectionTimeoutPeriod = tcpClientSettings.ConnectionTimeoutPeriod;
            _packetStreamFactory = tcpClientSettings.PacketStreamFactory;
        }

        public bool Start()
        {
            Trace.TraceInformation("[Client] Starting");
            _tcpClient = new TcpClient(_hostname, _port);
            _cancellationTokenSource = new CancellationTokenSource();
            ProcessServerAsync(_tcpClient, _cancellationTokenSource.Token);
            Trace.TraceInformation("[Client] Started");
            return true;
        }

        private async void ProcessServerAsync(TcpClient tcpClient, CancellationToken token)
        {
            if (ClientConnectedEvent != null)
            {
                ClientConnectedEvent(this, null);
            }

            Trace.TraceInformation("[Client] Connected to server");

            using (tcpClient)
            using (_networkStream = tcpClient.GetStream())
            using (var packetStream = _packetStreamFactory.New())
            {
                var buffer = new byte[4096];

                while (!token.IsCancellationRequested)
                {
                    var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(_connectionTimeoutPeriod), token);
                    var bytesTask = _networkStream.ReadAsync(buffer, 0, buffer.Length, token);
                    var completedTask = await Task.WhenAny(timeoutTask, bytesTask).ConfigureAwait(false);

                    if (completedTask == timeoutTask)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            Trace.TraceInformation("[Client] Server timed out");
                        }
                    }

                    var bytes = bytesTask.Result;
                    if (bytes == 0)
                    {
                        break;
                    }

                    if (MessageReceivedEvent == null)
                    {
                        continue;
                    }

                    var results = packetStream.ParseBytes(buffer.Take(bytes).ToArray());

                    if (results == null)
                    {
                        continue;
                    }

                    foreach (var result in results)
                    {
                        MessageReceivedEvent(this, result);
                    }
                }
            }

            if (ClientDisconnectedEvent != null)
            {
                ClientDisconnectedEvent(this, null);
            }

            Trace.TraceInformation("[Client] Diconnected from server");
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            Trace.TraceInformation("[Client] Stopping");
            _cancellationTokenSource.Cancel();
            Trace.TraceInformation("[Client] Stopped");
            return true;
        }

        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
            }
        }

        public async Task Send(byte[] message, CancellationToken cancellationToken)
        {
            await _networkStream.WriteAsync(message, 0, message.Count(), cancellationToken);
        }
    }
}