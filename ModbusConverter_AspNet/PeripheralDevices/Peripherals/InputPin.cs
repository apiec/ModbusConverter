using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Gpio;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class InputPin : InputPeripheral
    {
        private readonly GpioController _gpioController;
        private int _pinNumber;

        public InputPin(GpioController gpioController, ModbusServerWrapper modbusServerProxy)
            : base(modbusServerProxy)
        {
            _gpioController = gpioController;
            _gpioController.OpenPin(PinNumber, PinMode.Output);
        }

        ~InputPin()
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

        protected override bool[] ReadDataAsBools()
        {
            var data = ReadData();
            return new bool[] { data };
        }

        protected override ushort[] ReadDataAsUshorts()
        {
            var data = ReadData();
            var shortData = Convert.ToUInt16(data);
            return new ushort[] { shortData };
        }

        private bool ReadData()
        {
            AssertPinIsOpen();
            return (bool)_gpioController.Read(PinNumber);
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
