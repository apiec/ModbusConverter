using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.I2c;

namespace ModbusConverter.PeripheralDevices.AnalogIO
{
    //https://www.waveshare.com/w/upload/e/ed/PCF8591.pdf

    public class PCF8591Device
    {
        public enum InputMode : byte
        {
            SingleEnded_AIN0 = PCF8591Codes.ADC_AIN0,
            SingleEnded_AIN1 = PCF8591Codes.ADC_AIN1,
            SingleEnded_AIN2 = PCF8591Codes.ADC_AIN2,
            SingleEnded_AIN3 = PCF8591Codes.ADC_AIN3,

            Differential_AIN0_AIN3 = PCF8591Codes.ADC_DIFF_0_3,
            Differential_AIN1_AIN3 = PCF8591Codes.ADC_DIFF_1_3,
                                        
            Differential_AIN0_AIN1 = PCF8591Codes.ADC_DIFF_0_1,
            Differential_AIN2_AIN3 = PCF8591Codes.ADC_DIFF_2_3,
        }

        private struct PCF8591Codes
        {
            public const byte DAC_ENABLE = 0x40;
            public const byte ADC_AIN0 = 0x40;
            public const byte ADC_AIN1 = 0x41;
            public const byte ADC_AIN2 = 0x42;
            public const byte ADC_AIN3 = 0x43;

            public const byte ADC_DIFF_0_3 = 0x60;
            public const byte ADC_DIFF_1_3 = 0x61;

            public const byte ADC_DIFF_0_1 = 0x70;
            public const byte ADC_DIFF_2_3 = 0x71;
        }

        private readonly I2cDevice _i2cDevice;

        public PCF8591Device(I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        public void WriteToOutput(byte value)
        {
            var buffer = new byte[] { PCF8591Codes.DAC_ENABLE, value };
            _i2cDevice.Write(buffer);
        }

        public byte ReadInput(InputMode inputMode)
        {
            _i2cDevice.WriteByte((byte)inputMode);

            var bytes = new byte[2];
            _i2cDevice.Read(bytes);
            //need to read two bytes and ignore first because the first one contains old data
            return bytes[1];
        }
    }
}
