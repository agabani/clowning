using System;
using System.Threading;
using clowning.communicationsprotocol;
using clowning.communicationsprotocol.Json;
using clowning.communicationsprotocol.Json.Packets;
using clowning.communicationsprotocol.Json.Stream;
using clowning.communicationsprotocol.Models;

namespace clowning.master
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = Int32.Parse(args[0]);

            Console.WriteLine("Listening to port {0}", port);

            var protocol = new JsonProtocol();

            var server = new BlyncTcpServer(new TcpServerSettings
            {
                ConnectionTimeoutPeriod = 15000,
                PacketStreamFactory = new JsonPacketStreamFactory(),
                Port = port
            });

            server.ClientConnectedEvent += (sender, eventArgs) =>
            {
                var client = sender as TcpClientContext;
                if (client != null)
                {
                    Console.WriteLine("Client connected [{0}]", client.Id);

                    var redPacket = protocol.SetPacketContents(1, protocol.Serialize(new JsonInstructionPacket {Color = "red"}));
                    var greenPacket = protocol.SetPacketContents(1, protocol.Serialize(new JsonInstructionPacket { Color = "green" }));

                    for (int i = 0; i < 10; i++)
                    {
                        if (i%2 == 0)
                        {
                            client.Send(redPacket, new CancellationToken()).Wait();
                        }
                        else
                        {
                            client.Send(greenPacket, new CancellationToken()).Wait();
                        }
                        Thread.Sleep(2000);
                    }
                }
            };

            server.ClientDisconnectedEvent += (sender, eventArgs) =>
            {
                var client = sender as TcpClientContext;
                if (client != null)
                {
                    Console.WriteLine("Client disconnected [{0}]", client.Id);
                }
            };


            server.MessageReceivedEvent += (sender, bytes) =>
            {
                var client = sender as TcpClientContext;
                if (client != null)
                {
                    Console.WriteLine("Client disconnected [{0}]", client.Id);
                    var type = protocol.GetPacketType(bytes);
                    var length = protocol.GetPacketLength(bytes);
                    var content = protocol.GetPacketContents(bytes, length);
                    Console.WriteLine("    [{0}] {1}", type, content);
                }
            };

            try
            {
                server.Start();
                Thread.Sleep(400000);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                if (exception.InnerException != null)
                {
                    foreach (var innerExceptionMessage in exception.InnerException.Message)
                    {
                        Console.WriteLine("    {0}", innerExceptionMessage);
                    }
                }
            }
        }
    }
}
