using EasyModbus;
using ModbusConverter.Modbus;
using ModbusConverter.PeripheralDevices.Config;
using System;
using System.Device.Pwm;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class PwmPin : OutputPeripheral<float>
    {
        private readonly PwmChannel _pwmChannel;

        public PwmPin(PwmChannel pwmChannel, IModbusServerWrapper modbusServerWrapper)
            : base(modbusServerWrapper)
        {
            _pwmChannel = pwmChannel;
            _pwmChannel.Frequency = 11000;
            _pwmChannel.DutyCycle = 0;
            _pwmChannel.Start();
        }

        public int PinNumber { get; set; }

        public override PeripheralConfig GetConfig()
        {
            return new PwmPinConfig(this);
        }

        protected override float ReadValueFromBools(bool[] bools)
        {
            throw new NotSupportedException();
        }

        protected override float ReadValueFromRegisters(ushort[] registers)
        {
            return ModbusClient.ConvertRegistersToFloat(registers);
        }

        protected override void WriteValueToOutput(float value)
        {
            _pwmChannel.DutyCycle = value;
        }
    }
}
