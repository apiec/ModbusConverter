using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Pwm;
using EasyModbus;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class PwmPin : OutputPeripheral<double>
    {
        private readonly PwmChannel _pwmChannel;

        public PwmPin(PwmChannel pwmChannel, ModbusServerWrapper modbusServerProxy)
            : base(modbusServerProxy)
        {
            _pwmChannel = pwmChannel;
            _pwmChannel.DutyCycle = 0;
            _pwmChannel.Start();
        }

        protected override int DataLengthInBools => throw new NotSupportedException();

        protected override int DataLengthInRegisters => (int)Math.Ceiling((double)sizeof(double) / sizeof(ushort));

        protected override double ReadValueFromBools(bool[] bools)
        {
            throw new NotSupportedException();
        }

        protected override double ReadValueFromRegisters(ushort[] registers)
        {
            return ModbusClient.ConvertRegistersToDouble(registers);
        }


        protected override void WriteValueToOutput(double value)
        {
            CheckIfValueInBounds(value);
            _pwmChannel.DutyCycle = value;
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
