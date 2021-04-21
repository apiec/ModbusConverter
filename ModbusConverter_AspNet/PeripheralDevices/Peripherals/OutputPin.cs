using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Gpio;


namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class OutputPin : OutputPeripheral<bool>
    {
        private readonly GpioController _gpioController;
        private int _pinNumber;

        public OutputPin(GpioController gpioController, ModbusServerWrapper modbusServerProxy)
            : base(modbusServerProxy)
        {
            _gpioController = gpioController;
        }

        ~OutputPin()
        {
            if (_gpioController.IsPinOpen(PinNumber))
            {
                _gpioController.ClosePin(PinNumber);
            }
        }

        public int PinNumber
        {
            get => _pinNumber;
            set 
            {
                if (_gpioController.IsPinOpen(_pinNumber))
                {
                    _gpioController.ClosePin(_pinNumber);
                }
                _pinNumber = value;
                _gpioController.OpenPin(_pinNumber, PinMode.Output);
            }
        }

        protected override int DataLengthInBools => 1;

        protected override int DataLengthInRegisters => 1;

        protected override bool ReadValueFromBools(bool[] bools)
        {
            if (bools.Length != DataLengthInBools)
            {
                throw new ArgumentException("Wrong length of input array");
            }

            return bools[0];
        }

        protected override bool ReadValueFromRegisters(ushort[] registers)
        {
            if (registers.Length != DataLengthInRegisters)
            {
                throw new ArgumentException("Wrong length of input array");
            }

            return Convert.ToBoolean(registers[0]);
        }

        protected override void WriteValueToOutput(bool value)
        {
            AssertPinIsOpen();
            _gpioController.Write(PinNumber, value);
        }

        private void AssertPinIsOpen()
        {
            if (!_gpioController.IsPinOpen(PinNumber))
            {
                _gpioController.OpenPin(PinNumber, PinMode.Output);
            }
            else 
            if (_gpioController.GetPinMode(PinNumber) != PinMode.Output)
            {
                _gpioController.SetPinMode(PinNumber, PinMode.Output);
            }
        }
    }
}
