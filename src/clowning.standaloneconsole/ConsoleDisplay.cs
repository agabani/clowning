using System;
using clowning.blyncclient;
using clowning.standaloneconsole.Models;

namespace clowning.standaloneconsole
{
    internal class ConsoleDisplay
    {
        public void DisplayUsage(Options options)
        {
            Console.WriteLine(options.GetUsage());
        }

        public void DisplayDeviceNotFoundError()
        {
            Console.WriteLine("No devices found");
        }

        public void DisplayConnectedDevices(BlyncClient client)
        {
            Console.WriteLine("Number of devices connected: {0}", client.NumberOfDevices);

            for (var deviceId = 0; deviceId < client.NumberOfDevices; deviceId++)
            {
                Console.WriteLine("Device ID: {0}, Device Type: {1}", deviceId, client.GetDeviceType(deviceId));
            }
        }
    }
}