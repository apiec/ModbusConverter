using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.PeripheralDevices.AnalogIO;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class AnalogOutputChannel : OutputPeripheral<byte>
    {
        private readonly IAnalogIOController _analogIOController;

        public AnalogOutputChannel(IAnalogIOController analogIOController, ModbusServerWrapper modbusServerProxy)
            : base(modbusServerProxy)
        {
            _analogIOController = analogIOController;
        }

        public int Pcf8591Number { get; set; }

        protected override int DataLengthInBools => throw new NotSupportedException();

        protected override int DataLengthInRegisters => 1;

        protected override byte ReadValueFromBools(bool[] bools)
        {
            throw new NotSupportedException();
        }

        protected override byte ReadValueFromRegisters(ushort[] registers)
        {
            if (registers.Length != DataLengthInRegisters)
            {
                throw new ArgumentException("Wrong length of input array");
            }

            byte masked = (byte)(registers[0] & 0x00FF);
            return masked;
        }

        protected override void WriteValueToOutput(byte value)
        {
            _analogIOController.WriteToOutput(Pcf8591Number, value);
        }
    }
}
