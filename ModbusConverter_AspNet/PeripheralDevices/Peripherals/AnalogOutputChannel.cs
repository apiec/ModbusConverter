using ModbusConverter.Modbus;
using ModbusConverter.PeripheralDevices.AnalogIO;
using ModbusConverter.PeripheralDevices.Config;
using System;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class AnalogOutputChannel : OutputPeripheral<byte>
    {
        private readonly IAnalogIOController _analogIOController;

        public AnalogOutputChannel(IAnalogIOController analogIOController, IModbusServerWrapper modbusServerWrapper)
            : base(modbusServerWrapper)
        {
            _analogIOController = analogIOController;
        }

        public int PCF8591Number { get; set; }

        public override PeripheralConfig GetConfig()
        {
            return new AnalogOutputChannelConfig(this);
        }

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
            _analogIOController.WriteToOutput(PCF8591Number, value);
        }
    }
}
