using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.I2c;

namespace ModbusConverter.PeripheralDevices
{
    public class PCF8591InterfaceDevice : IInterfaceDevice
    {
        private struct PCF8591Codes
        {
            public const byte DAC_ENABLE = 0x40;
            public const byte ADC_CH0 = 0x40;
            public const byte ADC_CH1 = 0x41;
            public const byte ADC_CH2 = 0x42;
            public const byte ADC_CH3 = 0x43;
        }

        private readonly Dictionary<int, byte> _outputChannelToCodeMap = new Dictionary<int, byte>
        {
            {0, PCF8591Codes.ADC_CH0 },
            {1, PCF8591Codes.ADC_CH1 },
            {2, PCF8591Codes.ADC_CH2 },
            {3, PCF8591Codes.ADC_CH3 }
        };

        private readonly I2cDevice _i2cDevice;

        public PCF8591InterfaceDevice(I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        public void WriteToOutput(int outputChannel, byte value)
        {
            var buffer = new byte[] { PCF8591Codes.DAC_ENABLE, value };
            _i2cDevice.Write(buffer);
        }

        public byte ReadInput(int inputChannel)
        {
            var channelCode = _outputChannelToCodeMap[inputChannel];

            _i2cDevice.WriteByte(channelCode);

            //need to ignore one readbyte
            _i2cDevice.ReadByte();

            //return the other one
            return _i2cDevice.ReadByte();
        }



    }
}
