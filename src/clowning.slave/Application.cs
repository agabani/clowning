using System;
using System.Collections.Generic;
using System.Threading;
using clowning.blyncclient;
using clowning.communicationsprotocol;
using clowning.communicationsprotocol.Json;
using clowning.communicationsprotocol.Json.Packets;
using clowning.communicationsprotocol.Models;

namespace clowning.slave
{
    public class Application
    {
        private readonly BlyncClient _blyncClient;
        private bool _keepAlive;
        private readonly BlyncTcpClient _blyncTcpClient;
        private readonly InstructionExecuter _instructionExecuter;

        public Application(BlyncClient blyncClient, TcpClientSettings tcpClientSettings)
        {
            _blyncClient = blyncClient;
            _blyncTcpClient = new BlyncTcpClient(tcpClientSettings);
            _keepAlive = true;
            _instructionExecuter = new InstructionExecuter(_blyncClient);
        }

        public BlyncClient BlyncClient
        {
            get { return _blyncClient; }
        }

        public void Run()
        {

            _blyncTcpClient.ClientConnectedEvent += SendIdentityOnConnectedEvent();
            _blyncTcpClient.ClientDisconnectedEvent += ShutdownOnDisconnectedEvent();
            _blyncTcpClient.MessageReceivedEvent += ExcuteInstructionsOnMessageReceivedEvent();

            try
            {
                _blyncTcpClient.Start();
                while (_keepAlive)
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

        private BlyncTcpClient.OnMessageReceivedEvent ExcuteInstructionsOnMessageReceivedEvent()
        {
            return (sender, bytes) =>
            {
                Console.WriteLine("Message recieved");

                var protocol = new JsonProtocol();

                var type = protocol.GetPacketType(bytes);
                var length = protocol.GetPacketLength(bytes);
                var content = protocol.GetPacketContents(bytes, length);

                if (type == 1)
                {
                    var instruction = protocol.Deserialize<JsonInstructionPacket>(content);

                    _instructionExecuter.Execute(instruction);
                }
            };
        }

        private BlyncTcpClient.OnDisconnectedEvent ShutdownOnDisconnectedEvent()
        {
            _keepAlive = false;

            return (sender, eventArgs) =>
            {
                Console.WriteLine("Disconnected");
            };
        }

        private BlyncTcpClient.OnConnectedEvent SendIdentityOnConnectedEvent()
        {
            return (sender, eventArgs) =>
            {
                Console.WriteLine("Connected");

                var devices = new List<Device>();
                for (int i = 0; i < _blyncClient.NumberOfDevices; i++)
                {
                    devices.Add(new Device {DeviceId = i, DeviceType = _blyncClient.GetDeviceType(i).ToString()});
                }

                var status = new JsonIdentityPacket
                {
                    Name = Environment.MachineName,
                    Devices = devices
                };

                var protocol = new JsonProtocol();

                var json = protocol.Serialize(status);
                var packet = protocol.SetPacketContents(0, json);

                _blyncTcpClient.Send(packet, new CancellationToken()).Wait();
            };
        }
    }
}