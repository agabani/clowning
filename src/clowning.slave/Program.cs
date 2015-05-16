using System;
using clowning.blyncclient;
using clowning.communicationsprotocol.Json.Stream;
using clowning.communicationsprotocol.Models;

namespace clowning.slave
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostname = args[0];
            var port = Int32.Parse(args[1]);

            var blyncClient = new BlyncClient();
            var tcpClientSettings = new TcpClientSettings
            {
                ConnectionTimeoutPeriod = 30000,
                Hostname = hostname,
                Port = port,
                PacketStreamFactory = new JsonPacketStreamFactory()
            };

            if (blyncClient.NumberOfDevices == 0)
            {
                Console.WriteLine("No devices found");
                return;
            }

            Console.WriteLine("Devices found: {0}", blyncClient.NumberOfDevices);
            for (int id = 0; id < blyncClient.NumberOfDevices; id++)
            {
                Console.WriteLine("    [{0}] {1}", id, blyncClient.GetDeviceType(id));
            }

          
            Console.WriteLine("Connecting to {0}:{1}", hostname, port);

            new Application(blyncClient, tcpClientSettings).Run();
        }
    }
}
