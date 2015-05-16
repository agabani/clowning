using System.Collections.Generic;
using clowning.blyncclient;
using clowning.communicationsprotocol.Json.Packets;

namespace clowning.slave
{
    internal class InstructionExecuter
    {
        private readonly BlyncClient _blyncClient;

        public InstructionExecuter(BlyncClient blyncClient)
        {
            _blyncClient = blyncClient;
        }

        public void Execute(JsonInstructionPacket instruction)
        {
            var deviceIds = GetDeviceIds(instruction);

            if (instruction.Rgb != null)
            {
                SetRgbColor(deviceIds, instruction.Rgb);
            }

            if (!string.IsNullOrWhiteSpace(instruction.Color))
            {
                SetColor(instruction.Color.ToLower(), deviceIds);
            }

            if (instruction.Flash != null)
            {
                SetFlash(deviceIds, (bool) instruction.Flash);
            }

            if (instruction.Dim != null)
            {
                SetDim(deviceIds, (bool) instruction.Dim);
            }

            if (instruction.FlashSpeed != null)
            {
                SetFlashSpeed(deviceIds, (BlyncClient.Speed) instruction.FlashSpeed);
            }

            if (instruction.Reset == true)
            {
                ResetLight(deviceIds);
            }
        }

        private void SetColor(string lower, IEnumerable<int> deviceIds)
        {
            switch (lower)
            {
                case "red":
                    SetColor(deviceIds, BlyncClient.Color.Red);
                    break;
                case "green":
                    SetColor(deviceIds, BlyncClient.Color.Green);
                    break;
                case "blue":
                    SetColor(deviceIds, BlyncClient.Color.Blue);
                    break;
                case "cyan":
                    SetColor(deviceIds, BlyncClient.Color.Cyan);
                    break;
                case "magenta":
                    SetColor(deviceIds, BlyncClient.Color.Magenta);
                    break;
                case "yellow":
                    SetColor(deviceIds, BlyncClient.Color.Yellow);
                    break;
                case "white":
                    SetColor(deviceIds, BlyncClient.Color.White);
                    break;
                case "orange":
                    SetColor(deviceIds, BlyncClient.Color.Orange);
                    break;
                default:
                    _blyncClient.ResetLight(0);
                    break;
            }
        }

        private void SetColor(IEnumerable<int> deviceIds, BlyncClient.Color color)
        {
            foreach (var deviceId in deviceIds)
            {
                _blyncClient.SetColor(deviceId, color);
            }
        }

        private void ResetLight(IEnumerable<int> deviceIds)
        {
            foreach (var deviceId in deviceIds)
            {
                _blyncClient.ResetLight(deviceId);
            }
        }

        private void SetFlashSpeed(IEnumerable<int> deviceIds, BlyncClient.Speed flashSpeed)
        {
            foreach (var deviceId in deviceIds)
            {
                _blyncClient.SetFlashSpeed(deviceId, flashSpeed);
            }
        }

        private void SetDim(IEnumerable<int> deviceIds, bool enable)
        {
            foreach (var deviceId in deviceIds)
            {
                _blyncClient.SetDim(deviceId, enable);
            }
        }

        private void SetFlash(IEnumerable<int> deviceIds, bool enable)
        {
            foreach (var deviceId in deviceIds)
            {
                _blyncClient.SetFlash(deviceId, enable);
            }
        }

        private void SetRgbColor(IEnumerable<int> deviceIds, BlyncRgb blyncRgb)
        {
            foreach (var deviceId in deviceIds)
            {
                _blyncClient.SetColor(deviceId, blyncRgb.Red, blyncRgb.Green, blyncRgb.Blue);
            }
        }

        private IEnumerable<int> GetDeviceIds(JsonInstructionPacket instruction)
        {
            IEnumerable<int> deviceIds;
            if (instruction != null)
            {
                deviceIds = instruction.DeviceIds;
            }
            else
            {
                var list = new List<int>();
                for (var i = 0; i < _blyncClient.NumberOfDevices; i++)
                {
                    list.Add(i);
                }
                deviceIds = list;
            }
            return deviceIds;
        }
    }
}