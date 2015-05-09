using System;
using System.Collections.Generic;
using clowning.blyncclient;
using clowning.standaloneconsole.Models;

namespace clowning.standaloneconsole
{
    internal class OptionsExecuter
    {
        private readonly BlyncClient _blyncClient;

        public OptionsExecuter(BlyncClient blyncClient)
        {
            _blyncClient = blyncClient;
        }

        public bool Execute(Options options)
        {
            if (options.Quit)
            {
                return false;
            }

            if (options.Device == null)
            {
                options.Device = ProcessDevices(_blyncClient);
            }

            if (options.Color != null)
            {
                ProcessColor(_blyncClient, options);
            }

            if (options.Dim != null)
            {
                ProcessDim(_blyncClient, options);
            }

            if (options.Flash != null)
            {
                ProcessFlash(_blyncClient, options);
            }

            if (options.FlashSpeed != null)
            {
                ProcessFlashSpeed(_blyncClient, options);
            }

            if (options.Reset)
            {
                ProcessReset(_blyncClient, options);
            }

            return true;
        }

        private List<string> ProcessDevices(BlyncClient blyncClient)
        {
            var devices = new List<string>();
            for (var i = 0; i < blyncClient.NumberOfDevices; i++)
            {
                devices.Add(i.ToString());
            }
            return devices;
        }

        private void ProcessColor(BlyncClient blyncClient, Options options)
        {
            BlyncClient.Color color;
            switch (options.Color.ToLower())
            {
                case "red":
                    color = BlyncClient.Color.Red;
                    break;
                case "green":
                    color = BlyncClient.Color.Green;
                    break;
                case "blue":
                    color = BlyncClient.Color.Blue;
                    break;
                case "cyan":
                    color = BlyncClient.Color.Cyan;
                    break;
                case "magenta":
                    color = BlyncClient.Color.Magenta;
                    break;
                case "yellow":
                    color = BlyncClient.Color.Yellow;
                    break;
                case "white":
                    color = BlyncClient.Color.White;
                    break;
                case "orange":
                    color = BlyncClient.Color.Orange;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var device in options.Device)
            {
                blyncClient.SetColor(Int32.Parse(device), color);
            }
        }

        private void ProcessDim(BlyncClient blyncClient, Options options)
        {
            bool enable = options.Dim ?? false;

            foreach (var device in options.Device)
            {
                blyncClient.SetDim(Int32.Parse(device), enable);
            }
        }

        private void ProcessFlash(BlyncClient blyncClient, Options options)
        {
            var enable = options.Flash ?? false;

            foreach (var device in options.Device)
            {
                blyncClient.SetFlash(Int32.Parse(device), enable);
            }
        }

        private void ProcessFlashSpeed(BlyncClient blyncClient, Options options)
        {
            var flashSpeed = options.FlashSpeed ?? 1;

            BlyncClient.Speed speed;

            switch (flashSpeed)
            {
                case 1:
                    speed = BlyncClient.Speed.Low;
                    break;
                case 2:
                    speed = BlyncClient.Speed.Medium;
                    break;
                case 3:
                    speed = BlyncClient.Speed.High;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var device in options.Device)
            {
                blyncClient.SetFlashSpeed(Int32.Parse(device), speed);
            }
        }

        private void ProcessReset(BlyncClient blyncClient, Options options)
        {
            foreach (var device in options.Device)
            {
                blyncClient.ResetLight(Int32.Parse(device));
            }
        }
    }
}

/*
 *          usage.AppendLine("Usage: *.exe [-i id] [-c color] [-d enable] [-f enable] [-s speed]");
            usage.AppendLine("             [-z] [-v] [-q]");
            usage.AppendLine("");
            usage.AppendLine("Options:");
            usage.AppendLine("    -i id     Device id to target.");
            usage.AppendLine("    -c value  Change device color to one of following values:");
            usage.AppendLine("              Red,Green,Blue,Cyan,Magenta,Yellow,White,Orange");
            usage.AppendLine("    -d enable Dim light.");
            usage.AppendLine("    -f enable Flash light.");
            usage.AppendLine("    -s speed  Set flash speed to one of the following values:");
            usage.AppendLine("              Low,Medium,High");
            usage.AppendLine("    -z        Resets light.");
            usage.AppendLine("    -v        Verbose.");
            usage.AppendLine("    -q        Quit.");
            return usage.ToString();
*/