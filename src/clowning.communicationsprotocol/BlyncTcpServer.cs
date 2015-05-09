using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace clowning.communicationsprotocol
{
    public class BlyncTcpServer : IDisposable
    {
        private readonly TcpListener _tcpListener;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public event OnConnectedEvent ClientConnectedEvent;
        public event OnDisconnectedEvent ClientDisconnectedEvent;
        public event OnMessageReceivedEvent MessageReceivedEvent;

        public delegate void OnConnectedEvent(object sender, EventArgs args);

        public delegate void OnDisconnectedEvent(object sender, EventArgs args);

        public delegate void OnMessageReceivedEvent(object sender, string args);

        public BlyncTcpServer(int port)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
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
            Trace.TraceInformation("Listening started");
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

            if (ClientConnectedEvent != null)
            {
                ClientConnectedEvent(clientContext, EventArgs.Empty);
            }

            Trace.TraceInformation("New tcpClient ({0}) connected", clientIndex);

            using (tcpClient)
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[4096];

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                        var amountReadTask = networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        var completedTask = await Task.WhenAny(timeoutTask, amountReadTask).ConfigureAwait(false);

                        if (completedTask == timeoutTask)
                        {
                            var message = Encoding.UTF8.GetBytes("Client timed out");
                            await networkStream.WriteAsync(message, 0, message.Length, cancellationToken);
                        }

                        var amountRead = amountReadTask.Result;
                        if (amountRead == 0)
                        {
                            break;
                        }

                        if (MessageReceivedEvent != null)
                        {
                            MessageReceivedEvent(clientContext, Encoding.UTF8.GetString(buffer, 0, amountRead));
                        }

                        //await networkStream.WriteAsync(buffer, 0, amountRead, cancellationToken).ConfigureAwait(false);
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