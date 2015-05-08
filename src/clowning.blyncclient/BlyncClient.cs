using System;
using System.Collections.Generic;
using System.Linq;
using Blynclight;

namespace clowning.blyncclient
{
    public class BlyncClient : IDisposable
    {
        public enum Color
        {
            Red,
            Green,
            Blue,
            Cyan,
            Magenta,
            Yellow,
            White,
            Orange
        }

        public enum DeviceTypes
        {
            BlyncUsb10,
            BlyncUsb20,
            BlyncUsb30,
            BlyncUsb30S
        }

        public enum Speed
        {
            Low = 0x01,
            Medium = 0x02,
            High = 0x03
        }

        private readonly BlynclightController _blynclightController;
        private DeviceTypes[] _deviceTypes;

        public BlyncClient()
        {
            _blynclightController = new BlynclightController();
            NumberOfDevices = _blynclightController.InitBlyncDevices();
            _deviceTypes = IdentifyDeviceTypes();
        }

        public int NumberOfDevices { get; private set; }

        public void Dispose()
        {
            _blynclightController.CloseDevices(NumberOfDevices);
            _deviceTypes = null;
            NumberOfDevices = 0;
        }

        public DeviceTypes GetDeviceType(int deviceId)
        {
            if (_deviceTypes == null)
            {
                throw new InvalidOperationException();
            }

            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            return _deviceTypes[deviceId];
        }

        public IEnumerable<DeviceTypes> GetDeviceTypes()
        {
            if (_deviceTypes == null)
            {
                throw new InvalidOperationException();
            }

            return _deviceTypes.ToList();
        }

        private DeviceTypes[] IdentifyDeviceTypes()
        {
            var deviceTypes = new DeviceTypes[NumberOfDevices];

            for (var device = 0; device < NumberOfDevices; device++)
            {
                switch (_blynclightController.GetDeviceType(device))
                {
                    case 0x01:
                        deviceTypes[device] = DeviceTypes.BlyncUsb10;
                        break;
                    case 0x02:
                        deviceTypes[device] = DeviceTypes.BlyncUsb20;
                        break;
                    case 0x03:
                        deviceTypes[device] = DeviceTypes.BlyncUsb30;
                        break;
                    case 0x04:
                        deviceTypes[device] = DeviceTypes.BlyncUsb30S;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return deviceTypes;
        }

        public bool TurnOnLight(int deviceNumber, Color color)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            switch (color)
            {
                case Color.Red:
                    return _blynclightController.TurnOnRedLight(deviceNumber);
                case Color.Green:
                    return _blynclightController.TurnOnGreenLight(deviceNumber);
                case Color.Blue:
                    return _blynclightController.TurnOnBlueLight(deviceNumber);
                case Color.Cyan:
                    return _blynclightController.TurnOnCyanLight(deviceNumber);
                case Color.Magenta:
                    return _blynclightController.TurnOnMagentaLight(deviceNumber);
                case Color.Yellow:
                    return _blynclightController.TurnOnYellowLight(deviceNumber);
                case Color.White:
                    return _blynclightController.TurnOnWhiteLight(deviceNumber);
                case Color.Orange:
                    if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30 &&
                        _deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
                    {
                        throw new InvalidOperationException();
                    }
                    return _blynclightController.TurnOnOrangeLight(deviceNumber);
                default:
                    throw new ArgumentOutOfRangeException("color");
            }
        }

        public bool TurnOnLight(int deviceNumber, int red, int green, int blue)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30 &&
                _deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            if (IsInvalidColorIntensity(red))
            {
                throw new ArgumentOutOfRangeException("red");
            }

            if (IsInvalidColorIntensity(green))
            {
                throw new ArgumentOutOfRangeException("green");
            }

            if (IsInvalidColorIntensity(blue))
            {
                throw new ArgumentOutOfRangeException("blue");
            }

            return _blynclightController.TurnOnRGBLights(deviceNumber, (byte) red, (byte) green, (byte) blue);
        }

        public bool ResetLight(int deviceNumber)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            return _blynclightController.ResetLight(deviceNumber);
        }

        public bool DimLight(int deviceNumber, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30 &&
                _deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.SetLightDim(deviceNumber)
                : _blynclightController.ClearLightDim(deviceNumber);
        }

        public bool Flash(int deviceNumber, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30 &&
                _deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.StartLightFlash(deviceNumber)
                : _blynclightController.StopLightFlash(deviceNumber);
        }

        public bool SetFlashSpeed(int deviceNumber, Speed speed)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30 &&
                _deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            if (IsInvalidSpeed(speed))
            {
                throw new ArgumentOutOfRangeException("speed");
            }

            return _blynclightController.SelectLightFlashSpeed(deviceNumber, (byte) speed);
        }

        public bool PlayMusic(int deviceNumber, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.StartMusicPlay(deviceNumber)
                : _blynclightController.StopMusicPlay(deviceNumber);
        }

        public bool SelectMusic(int deviceNumber, int sound)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            if (IsNotValidSound(sound))
            {
                throw new ArgumentOutOfRangeException("sound");
            }

            return _blynclightController.SelectMusicToPlay(deviceNumber, (byte) sound);
        }

        public bool RepeatMusic(int deviceNumber, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.SetMusicRepeat(deviceNumber)
                : _blynclightController.ClearMusicRepeat(deviceNumber);
        }

        public bool MusicVolume(int deviceNumber, int volumeLevel)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            if (IsNotValidVolumeLevel(volumeLevel))
            {
                throw new ArgumentOutOfRangeException("volumeLevel");
            }

            return _blynclightController.SetMusicVolume(deviceNumber, (byte) volumeLevel);
        }

        public bool MuteVolume(int deviceNumber, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceNumber))
            {
                throw new ArgumentOutOfRangeException("deviceNumber");
            }

            if (_deviceTypes[deviceNumber] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.SetVolumeMute(deviceNumber)
                : _blynclightController.ClearVolumeMute(deviceNumber);
        }

        private static bool IsInvalidSpeed(Speed speed)
        {
            var values = Enum.GetValues(typeof (Speed)).Cast<Speed>();
            return !values.Contains(speed);
        }

        private bool IsInvalidDeviceNumber(int deviceNumber)
        {
            return deviceNumber >= NumberOfDevices || deviceNumber < 0;
        }

        private static bool IsInvalidColorIntensity(int intensity)
        {
            return intensity > 255 || intensity < 0;
        }

        private bool IsNotValidSound(int sound)
        {
            return sound >= 1 && sound <= 10;
        }

        private bool IsNotValidVolumeLevel(int volumeLevel)
        {
            return volumeLevel >= 1 && volumeLevel <= 10;
        }
    }
}