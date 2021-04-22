using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.PeripheralDevices.AnalogIO;
using System.Device.Gpio;
using System.Device.Pwm;
using Microsoft.Extensions.Configuration;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class PeripheralsFactory
    {
        private readonly ModbusServerWrapper _modbusServerProxy;
        private readonly GpioController _gpioController;
        private readonly IAnalogIOController _analogIOController;
        private readonly List<int> _pwmPins;

        public PeripheralsFactory(
            ModbusServerWrapper modbusServerProxy, 
            GpioController gpioController,
            IAnalogIOController analogIOController,
            IConfiguration configuration)
        {
            _modbusServerProxy = modbusServerProxy;
            _gpioController = gpioController;
            _analogIOController = analogIOController;
            _pwmPins = new List<int>();
            configuration.GetSection("PWMPins").Bind(_pwmPins);
        }

        public AnalogInputChannel CreateAnalogInputChannel(int pcf8591Number, PCF8591Device.InputMode inputMode)
        {
            var analogInputChannel = new AnalogInputChannel(_analogIOController, _modbusServerProxy);
            analogInputChannel.PCF8591Number = pcf8591Number;
            analogInputChannel.InputMode = inputMode;

            return analogInputChannel;
        }

        public AnalogOutputChannel CreateAnalogOutputChannel(int pcf8591Number)
        {
            var analogOutputChannel = new AnalogOutputChannel(_analogIOController, _modbusServerProxy);
            analogOutputChannel.Pcf8591Number = pcf8591Number;

            return analogOutputChannel;
        }

        public InputPin CreateInputPin(int pinNumber)
        {
            var inputPin = new InputPin(_gpioController, _modbusServerProxy);
            inputPin.PinNumber = pinNumber;

            return inputPin;
        }

        public OutputPin CreateOutputPin(int pinNumber)
        {
            var outputPin = new OutputPin(_gpioController, _modbusServerProxy);
            outputPin.PinNumber = pinNumber;

            return outputPin;
        }

        public PwmPin CreatePwmPin(int pinNumber)
        {
            var index = _pwmPins.FindIndex(pin => pin == pinNumber);
            if (index == -1)
            {
                throw new ArgumentException("Given pin number is not a defined pwm pin");
            }

            //using raspi built in pwm chip, channels are 0 and 1
            var pwmChannel = PwmChannel.Create(chip: 0, channel: index, dutyCyclePercentage: 0);
            var pwmPin = new PwmPin(pwmChannel, _modbusServerProxy);

            return pwmPin;
        }
    }
}
