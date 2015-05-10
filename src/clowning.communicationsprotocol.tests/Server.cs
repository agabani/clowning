using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using clowning.communicationsprotocol.Json;
using clowning.communicationsprotocol.Json.Stream;
using clowning.communicationsprotocol.Models;
using NUnit.Framework;

namespace clowning.communicationsprotocol.tests
{
    [TestFixture]
    public class Server
    {
        private const int Port = 8000;

        [Test]
        public void Should_be_able_to_start_server()
        {
            var server = new BlyncTcpServer(new TcpServerSettings {Port = Port});
            server.Start();
            server.Stop();
        }

        [Test]
        public void Should_be_able_to_send_message_from_server()
        {
            var server = new BlyncTcpServer(new TcpServerSettings
            {
                Port = Port
            });

            server.ClientConnectedEvent += (sender, args) =>
            {
                var tcpClientContext = sender as TcpClientContext;
                if (tcpClientContext != null)
                {
                    var message = Encoding.UTF8.GetBytes("MyEvent");
                    tcpClientContext.Send(message, new CancellationToken()).Wait();
                }
                server.Stop();
            };
            
            server.Start();

            var client = new TcpClient("127.0.0.1", Port);
            var stream = client.GetStream();
            var buffer = new Byte[4096];
            var bytes = stream.Read(buffer, 0, buffer.Length);
            var response = Encoding.UTF8.GetString(buffer, 0, bytes);

            Assert.That(response, Is.EqualTo("MyEvent"));
        }

        [Test]
        public void Should_be_notified_when_client_sends_a_message()
        {
            var expectedMessage = new JsonProtocol().SetPacketContents(0, "Hello world!");

            var server = new BlyncTcpServer(new TcpServerSettings
            {
                Port = Port,
                PacketStreamFactory = new JsonPacketStreamFactory()
            });

            byte[] actualMessage = null;

            server.MessageReceivedEvent += (sender, args) =>
            {
                var context = sender as TcpClientContext;
                if (context != null)
                {
                    actualMessage = args;
                }
                server.Stop();
            };

            server.Start();

            var tcpClient = new TcpClient("127.0.0.1", Port);
            var stream = tcpClient.GetStream();
            stream.Write(expectedMessage, 0, expectedMessage.Length);

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(1));
            while (!timeoutTask.IsCompleted && actualMessage == null)
            {
                // Wait
            }

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void Should_send_timeout_if_client_is_inactive_for_50_millisseconds()
        {
            var stopwatch = new Stopwatch();

            var server = new BlyncTcpServer(new TcpServerSettings
            {
                Port = Port,
                ConnectionTimeoutPeriod = 50,
                PacketStreamFactory = new JsonPacketStreamFactory()
            });

            server.ClientDisconnectedEvent += (sender, args) => server.Stop();

            server.Start();

            var client = new TcpClient("127.0.0.1", Port);
            var stream = client.GetStream();
            var buffer = new byte[4096];

            stopwatch.Start();
            var bytes = stream.Read(buffer, 0, buffer.Length);
            stopwatch.Stop();

            var response = Encoding.UTF8.GetString(buffer, 0, bytes);
            Assert.That(response, Is.EqualTo("Client timed out"));
            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(50).And.LessThan(70));
        }
    }
}