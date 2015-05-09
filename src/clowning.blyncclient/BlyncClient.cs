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

        public bool TurnOnLight(int deviceId, Color color)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            switch (color)
            {
                case Color.Red:
                    return _blynclightController.TurnOnRedLight(deviceId);
                case Color.Green:
                    return _blynclightController.TurnOnGreenLight(deviceId);
                case Color.Blue:
                    return _blynclightController.TurnOnBlueLight(deviceId);
                case Color.Cyan:
                    return _blynclightController.TurnOnCyanLight(deviceId);
                case Color.Magenta:
                    return _blynclightController.TurnOnMagentaLight(deviceId);
                case Color.Yellow:
                    return _blynclightController.TurnOnYellowLight(deviceId);
                case Color.White:
                    return _blynclightController.TurnOnWhiteLight(deviceId);
                case Color.Orange:
                    if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30 &&
                        _deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
                    {
                        throw new InvalidOperationException();
                    }
                    return _blynclightController.TurnOnOrangeLight(deviceId);
                default:
                    throw new ArgumentOutOfRangeException("color");
            }
        }

        public bool TurnOnLight(int deviceId, int red, int green, int blue)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30 &&
                _deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
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

            return _blynclightController.TurnOnRGBLights(deviceId, (byte) red, (byte) green, (byte) blue);
        }

        public bool ResetLight(int deviceId)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            return _blynclightController.ResetLight(deviceId);
        }

        public bool DimLight(int deviceId, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30 &&
                _deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.SetLightDim(deviceId)
                : _blynclightController.ClearLightDim(deviceId);
        }

        public bool Flash(int deviceId, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30 &&
                _deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.StartLightFlash(deviceId)
                : _blynclightController.StopLightFlash(deviceId);
        }

        public bool SetFlashSpeed(int deviceId, Speed speed)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30 &&
                _deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            if (IsInvalidSpeed(speed))
            {
                throw new ArgumentOutOfRangeException("speed");
            }

            return _blynclightController.SelectLightFlashSpeed(deviceId, (byte) speed);
        }

        public bool PlayMusic(int deviceId, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.StartMusicPlay(deviceId)
                : _blynclightController.StopMusicPlay(deviceId);
        }

        public bool SelectMusic(int deviceId, int sound)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            if (IsNotValidSound(sound))
            {
                throw new ArgumentOutOfRangeException("sound");
            }

            return _blynclightController.SelectMusicToPlay(deviceId, (byte) sound);
        }

        public bool RepeatMusic(int deviceId, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.SetMusicRepeat(deviceId)
                : _blynclightController.ClearMusicRepeat(deviceId);
        }

        public bool MusicVolume(int deviceId, int volumeLevel)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            if (IsNotValidVolumeLevel(volumeLevel))
            {
                throw new ArgumentOutOfRangeException("volumeLevel");
            }

            return _blynclightController.SetMusicVolume(deviceId, (byte) volumeLevel);
        }

        public bool MuteVolume(int deviceId, bool enable)
        {
            if (IsInvalidDeviceNumber(deviceId))
            {
                throw new ArgumentOutOfRangeException("deviceId");
            }

            if (_deviceTypes[deviceId] != DeviceTypes.BlyncUsb30S)
            {
                throw new InvalidOperationException();
            }

            return enable
                ? _blynclightController.SetVolumeMute(deviceId)
                : _blynclightController.ClearVolumeMute(deviceId);
        }

        private static bool IsInvalidSpeed(Speed speed)
        {
            var values = Enum.GetValues(typeof (Speed)).Cast<Speed>();
            return !values.Contains(speed);
        }

        private bool IsInvalidDeviceNumber(int deviceId)
        {
            return deviceId >= NumberOfDevices || deviceId < 0;
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