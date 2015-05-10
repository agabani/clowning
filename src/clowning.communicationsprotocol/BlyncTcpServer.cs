using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using clowning.communicationsprotocol.Models;

namespace clowning.communicationsprotocol
{
    public class BlyncTcpServer : IDisposable
    {
        private readonly TcpListener _tcpListener;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly int _connectionTimeoutPeriod;

        public event OnConnectedEvent ClientConnectedEvent;
        public event OnDisconnectedEvent ClientDisconnectedEvent;
        public event OnMessageReceivedEvent MessageReceivedEvent;

        public delegate void OnConnectedEvent(object sender, EventArgs args);

        public delegate void OnDisconnectedEvent(object sender, EventArgs args);

        public delegate void OnMessageReceivedEvent(object sender, byte[] args);

        public BlyncTcpServer(TcpServerSettings tcpServerSettings)
        {
            _tcpListener = new TcpListener(IPAddress.Any, tcpServerSettings.Port);
            _connectionTimeoutPeriod = tcpServerSettings.ConnectionTimeoutPeriod > 0
                ? tcpServerSettings.ConnectionTimeoutPeriod
                : 15000;
        }

        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
            }
        }

        public bool Start()
        {
            _tcpListener.Start();
            AcceptClientsAsync(_tcpListener, _cancellationTokenSource.Token);
            Trace.TraceInformation("Listening started on {0}", _tcpListener.LocalEndpoint);
            return true;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _tcpListener.Stop();
            Trace.TraceInformation("Listening stopped");
        }

        private async void AcceptClientsAsync(TcpListener tcpListener, CancellationToken cancellationToken)
        {
            var clientCounter = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                clientCounter++;

                ProcessClientAsync(tcpClient, clientCounter, cancellationToken);
            }
        }

        private async void ProcessClientAsync(TcpClient tcpClient, int clientIndex, CancellationToken cancellationToken)
        {
            var clientContext = new TcpClientContext(clientIndex, tcpClient);

            var jsonPacketStream = new JsonPacketStream();

            if (ClientConnectedEvent != null)
            {
                ClientConnectedEvent(clientContext, EventArgs.Empty);
            }

            Trace.TraceInformation("New client ({0}) connected", clientIndex);

            using (tcpClient)
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[4096];

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(_connectionTimeoutPeriod),
                            cancellationToken);
                        var bytesTask = networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        var completedTask = await Task.WhenAny(timeoutTask, bytesTask).ConfigureAwait(false);

                        if (completedTask == timeoutTask)
                        {
                            var message = Encoding.UTF8.GetBytes("Client timed out");
                            await networkStream.WriteAsync(message, 0, message.Length, cancellationToken);
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

                        var results = jsonPacketStream.FeedBytes(buffer.Take(bytes).ToArray());

                        if (results == null)
                        {
                            continue;
                        }

                        foreach (var result in results)
                        {
                            MessageReceivedEvent(clientContext, result);
                        }
                    }
                }
            }

            if (ClientDisconnectedEvent != null)
            {
                ClientDisconnectedEvent(clientContext, EventArgs.Empty);
            }

            Trace.TraceInformation("Client ({0}) disconnected", clientIndex);
        }
    }
}