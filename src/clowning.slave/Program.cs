using System;
using System.Collections.Generic;
using System.Threading;
using clowning.blyncclient;
using clowning.communicationsprotocol;
using clowning.communicationsprotocol.Json;
using clowning.communicationsprotocol.Json.Packets;
using clowning.communicationsprotocol.Json.Stream;
using clowning.communicationsprotocol.Models;

namespace clowning.slave
{
    class Program
    {
        static void Main(string[] args)
        {
            var blync = new BlyncClient();

            if (blync.NumberOfDevices == 0)
            {
                Console.WriteLine("No devices found");
                return;
            }

            Console.WriteLine("Devices found: {0}", blync.NumberOfDevices);
            for (int id = 0; id < blync.NumberOfDevices; id++)
            {
                Console.WriteLine("    [{0}] {1}", id, blync.GetDeviceType(id));
            }

            var hostname = args[0];
            var port = Int32.Parse(args[1]);

            Console.WriteLine("Connecting to {0}:{1}", hostname, port);

            var client = new BlyncTcpClient(new TcpClientSettings
            {
                ConnectionTimeoutPeriod = 30000,
                Hostname = hostname,
                Port = port,
                PacketStreamFactory = new JsonPacketStreamFactory()
            });

            client.ClientConnectedEvent += (sender, eventArgs) =>
            {
                Console.WriteLine("Connected");

                var devices = new List<Device>();
                for (int i = 0; i < blync.NumberOfDevices; i++)
                {
                    devices.Add(new Device{DeviceId = i, DeviceType = blync.GetDeviceType(i).ToString()});
                }

                var status = new JsonIdentityPacket
                {
                    Name = Environment.MachineName,
                    Devices = devices
                };

                var protocol = new JsonProtocol();

                var json = protocol.Serialize(status);
                var packet = protocol.SetPacketContents(0, json);

                client.Send(packet, new CancellationToken()).Wait();
            };

            bool keepAlive = true;

            client.ClientDisconnectedEvent += (sender, eventArgs) =>
            {
                Console.WriteLine("Disconnected");
                keepAlive = false;
            };

            client.MessageReceivedEvent += (sender, bytes) =>
            {
                Console.WriteLine("Message recieved");

                var protocol = new JsonProtocol();

                var type = protocol.GetPacketType(bytes);
                var length = protocol.GetPacketLength(bytes);
                var content = protocol.GetPacketContents(bytes, length);

                if (type == 1)
                {
                    var instruction = protocol.Deserialize<JsonInstructionPacket>(content);

                    if (!string.IsNullOrWhiteSpace(instruction.Color))
                    {
                        switch (instruction.Color.ToLower())
                        {
                            case "red":
                                blync.SetColor(0, BlyncClient.Color.Red);
                                break;
                            case "green":
                                blync.SetColor(0, BlyncClient.Color.Green);
                                break;
                            default:
                                blync.ResetLight(0);
                                break;
                        }
                    }
                }
            };

            try
            {
                client.Start();
                while (keepAlive)
                {
                    Thread.Sleep(100);
                }
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
