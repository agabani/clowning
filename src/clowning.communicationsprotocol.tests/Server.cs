using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace clowning.communicationsprotocol.tests
{
    [TestFixture]
    public class Server
    {
        [Test]
        public void Should_be_able_to_start_server()
        {
            var server = new BlyncTcpServer(8000);
            server.Start();
            server.Stop();
        }

        [Test]
        public void Should_be_able_to_send_message_from_server()
        {
            var server = new BlyncTcpServer(8000);
            server.ClientConnectedEvent += ServerOnClientConnectedEvent;
            server.Start();
            var client = new TcpClient("127.0.0.1", 8000);
            var stream = client.GetStream();
            var buffer = new Byte[4096];
            var bytes = stream.Read(buffer, 0, buffer.Length);
            var response = Encoding.UTF8.GetString(buffer, 0, bytes);
            server.Stop();
            Assert.That(response, Is.EqualTo("MyEvent"));
        }

        [Test]
        public void Should_be_notified_when_client_sends_a_message()
        {
            var called = false;
            var server = new BlyncTcpServer(8000);
            server.MessageReceivedEvent += ServerOnMessageReceivedEvent;
            server.MessageReceivedEvent += (o, e) => called = true;
            server.Start();
            var tcpClient = new TcpClient("127.0.0.1", 8000);
            var stream = tcpClient.GetStream();
            var message = Encoding.UTF8.GetBytes("Hello world!");
            stream.Write(message, 0, message.Length);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(1));
            while (!timeoutTask.IsCompleted && called == false)
            {
                // Do nothing
            }
            Assert.That(called, Is.True);
        }

        [Test]
        public void Should_send_timeout_if_client_is_inactive_for_15_seconds()
        {
            var stopwatch = new Stopwatch();
            var server = new BlyncTcpServer(8000);
            server.Start();
            var client = new TcpClient("127.0.0.1", 8000);
            var stream = client.GetStream();
            var buffer = new byte[4096];
            stopwatch.Start();
            var bytes = stream.Read(buffer, 0, buffer.Length);
            stopwatch.Stop();
            var response = Encoding.UTF8.GetString(buffer, 0, bytes);
            Assert.That(response, Is.EqualTo("Client timed out"));
            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThan(14500).And.LessThan(15500));
        }

        private void ServerOnMessageReceivedEvent(object sender, string args)
        {
            var context = sender as TcpClientContext;
            Assert.That(context, Is.Not.Null);
            if (context != null) Assert.That(context.Id, Is.EqualTo(1));
        }

        private void ServerOnClientConnectedEvent(object sender, EventArgs args)
        {
            var tcpClientContext = sender as TcpClientContext;
            if (tcpClientContext != null)
            {
                tcpClientContext.Send("MyEvent", new CancellationToken()).Wait();
            }
        }
    }
}