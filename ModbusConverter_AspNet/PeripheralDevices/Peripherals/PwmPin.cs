using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Pwm;
using EasyModbus;
using System.Device.Gpio;
using ModbusConverter.PeripheralDevices.Config;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class PwmPin : OutputPeripheral<byte>
    {
        private readonly PwmChannel _pwmChannel;

        public PwmPin(PwmChannel pwmChannel, IModbusServerWrapper modbusServerProxy)
            : base(modbusServerProxy)
        {
            _pwmChannel = pwmChannel;
            _pwmChannel.Frequency = 11000;
            _pwmChannel.DutyCycle = 0;
            _pwmChannel.Start();
        }

        public int PinNumber { get; set; }

        public override int DataLengthInBools => throw new NotSupportedException();

        public override int DataLengthInRegisters => (int)Math.Ceiling((double)sizeof(byte) / sizeof(ushort));

        public override PeripheralConfig GetConfig()
        {
            return new PwmPinConfig(this);
        }

        protected override byte ReadValueFromBools(bool[] bools)
        {
            throw new NotSupportedException();
        }

        protected override byte ReadValueFromRegisters(ushort[] registers)
        {
            return (byte)registers[0];
        }

        protected override void WriteValueToOutput(byte value)
        {
            _pwmChannel.DutyCycle = (double)value / byte.MaxValue;
        }

        private void CheckIfValueInBounds(double value)
        {
            if (value < 0 || value > 1.0)
            {
                throw new ArgumentOutOfRangeException("Pwm duty cycle value has to be in range 0-1");
            }
        }
    }
}
