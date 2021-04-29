using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.PeripheralDevices.AnalogIO;
using System.Device.Gpio;
using System.Device.Pwm;
using Microsoft.Extensions.Configuration;
using ModbusConverter.PeripheralDevices.Config;
using ModbusConverter.Modbus;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class PeripheralsFactory : IPeripheralsFactory
    {
        private readonly IModbusServerWrapper _modbusServerWrapper;
        private readonly GpioController _gpioController;
        private readonly IAnalogIOController _analogIOController;
        private readonly List<int> _pwmPins;

        public PeripheralsFactory(
            IModbusServerWrapper modbusServerWrapper,
            GpioController gpioController,
            IAnalogIOController analogIOController,
            IConfiguration configuration)
        {
            _modbusServerWrapper = modbusServerWrapper;
            _gpioController = gpioController;
            _analogIOController = analogIOController;
            _pwmPins = new List<int>();
            configuration.GetSection("PWMPins").Bind(_pwmPins);
        }

        public IPeripheral CreateFromConfig(PeripheralConfig peripheralConfig)
        {
            if (peripheralConfig is null)
            {
                throw new ArgumentNullException(nameof(peripheralConfig));
            }

            try
            {
                IPeripheral peripheral;
                switch (peripheralConfig)
                {
                    case AnalogInputChannelConfig config:
                        var inputMode = Enum.Parse(typeof(PCF8591Device.InputMode), config.InputMode);
                        peripheral = CreateAnalogInputChannel(config.PCF8591Number, (PCF8591Device.InputMode)inputMode);
                        break;
                    case AnalogOutputChannelConfig config:
                        peripheral = CreateAnalogOutputChannel(config.PCF8591Number);
                        break;
                    case InputPinConfig config:
                        peripheral = CreateInputPin(config.PinNumber);
                        break;
                    case OutputPinConfig config:
                        peripheral = CreateOutputPin(config.PinNumber);
                        break;
                    case PwmPinConfig config:
                        peripheral = CreatePwmPin(config.PinNumber);
                        break;
                    default:
                        throw new ArgumentException(nameof(peripheralConfig));
                }

                peripheral.Name = peripheralConfig.Name;
                peripheral.RegisterAddress = peripheralConfig.RegisterAddress;
            
                var registerType = Enum.Parse(typeof(ModbusRegisterType), peripheralConfig.RegisterType);
                peripheral.RegisterType = (ModbusRegisterType)registerType;

                return peripheral;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public AnalogInputChannel CreateAnalogInputChannel(int pcf8591Number, PCF8591Device.InputMode inputMode)
        {
            var analogInputChannel = new AnalogInputChannel(_analogIOController, _modbusServerWrapper);
            analogInputChannel.PCF8591Number = pcf8591Number;
            analogInputChannel.InputMode = inputMode;

            return analogInputChannel;
        }

        public AnalogOutputChannel CreateAnalogOutputChannel(int pcf8591Number)
        {
            var analogOutputChannel = new AnalogOutputChannel(_analogIOController, _modbusServerWrapper);
            analogOutputChannel.PCF8591Number = pcf8591Number;

            return analogOutputChannel;
        }

        public InputPin CreateInputPin(int pinNumber)
        {
            var inputPin = new InputPin(_gpioController, _modbusServerWrapper);
            inputPin.PinNumber = pinNumber;

            return inputPin;
        }

        public OutputPin CreateOutputPin(int pinNumber)
        {
            var outputPin = new OutputPin(_gpioController, _modbusServerWrapper);
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
            var pwmPin = new PwmPin(pwmChannel, _modbusServerWrapper);
            pwmPin.PinNumber = pinNumber;

            return pwmPin;
        }
    }
}
