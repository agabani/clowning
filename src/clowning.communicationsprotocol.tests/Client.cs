using System;
using System.Threading;
using System.Threading.Tasks;
using clowning.communicationsprotocol.Json;
using clowning.communicationsprotocol.Json.Stream;
using clowning.communicationsprotocol.Models;
using NUnit.Framework;

namespace clowning.communicationsprotocol.tests
{
    [TestFixture]
    public class Client
    {
        private const int Port = 7999;

        [Test]
        public void Should_be_able_to_connect_to_server()
        {
            var server = new BlyncTcpServer(new TcpServerSettings
            {
                ConnectionTimeoutPeriod = 5000,
                PacketStreamFactory = new JsonPacketStreamFactory(),
                Port = Port
            });
            var client = new BlyncTcpClient(new TcpClientSettings
            {
                ConnectionTimeoutPeriod = 10000,
                Hostname = "127.0.0.1",
                PacketStreamFactory = new JsonPacketStreamFactory(),
                Port = Port
            });

            bool serverConnected = false;
            bool severDisconnected = false;
            bool clientConnected = false;
            bool clientDisconnected = false;

            server.ClientConnectedEvent += (sender, args) => serverConnected = true;
            server.ClientDisconnectedEvent += (sender, args) =>
            {
                severDisconnected = true;
                server.Stop();
            };
            client.ClientConnectedEvent += (sender, args) =>
            {
                clientConnected = true;
                client.Stop();
            };
            client.ClientDisconnectedEvent += (sender, args) => clientDisconnected = true;

            server.Start();
            client.Start();

            var delay = Task.Delay(TimeSpan.FromSeconds(1));
            while (!delay.IsCompleted
                   && !(serverConnected && severDisconnected && clientConnected && clientDisconnected))
            {
                // Wait
            }

            Assert.That(serverConnected, Is.True);
            Assert.That(severDisconnected, Is.True);
            Assert.That(clientConnected, Is.True);
            Assert.That(clientDisconnected, Is.True);
        }

        [Test]
        public void Should_be_able_to_send_packet_to_server()
        {
            var packetProtocol = new JsonProtocol();

            var server = new BlyncTcpServer(new TcpServerSettings
            {
                ConnectionTimeoutPeriod = 5000,
                PacketStreamFactory = new JsonPacketStreamFactory(),
                Port = Port
            });
            var client = new BlyncTcpClient(new TcpClientSettings
            {
                ConnectionTimeoutPeriod = 10000,
                Hostname = "127.0.0.1",
                PacketStreamFactory = new JsonPacketStreamFactory(),
                Port = Port
            });

            byte[] actualMessage = null;

            server.MessageReceivedEvent += (sender, args) =>
            {
                actualMessage = args;
                client.Stop();
                server.Stop();
            };

            var expectedMessage = packetProtocol.SetPacketContents(0, "Hello World!");

            server.Start();
            client.Start();
            client.Send(expectedMessage, new CancellationToken()).Wait();

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(1));
            while (!timeoutTask.IsCompleted && actualMessage == null)
            {
                // Wait
            }

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void Should_be_able_to_recieve_packet_from_server()
        {
            var packetProtocol = new JsonProtocol();

            var server = new BlyncTcpServer(new TcpServerSettings
            {
                ConnectionTimeoutPeriod = 5000,
                PacketStreamFactory = new JsonPacketStreamFactory(),
                Port = Port
            });
            var client = new BlyncTcpClient(new TcpClientSettings
            {
                ConnectionTimeoutPeriod = 10000,
                Hostname = "127.0.0.1",
                PacketStreamFactory = new JsonPacketStreamFactory(),
                Port = Port
            });

            var expectedMessage = packetProtocol.SetPacketContents(0, "Hello World!");

            server.ClientConnectedEvent += (sender, args) =>
            {
                var clientContext = sender as TcpClientContext;
                if (clientContext != null)
                {
                    clientContext.Send(expectedMessage, new CancellationToken()).Wait();
                }
            };

            byte[] actualMessage = null;
            client.MessageReceivedEvent += (sender, args) =>
            {
                actualMessage = args;
                client.Stop();
                server.Stop();
            };

            server.Start();
            client.Start();

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(1));
            while (!timeoutTask.IsCompleted && actualMessage == null)
            {
                // Wait
            }

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }
    }
}