using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Gpio;
using System.Device.Pwm;

namespace ModbusConverter.PeripheralDevices
{
    public enum DigitalInputPin : int
    {
    }

    public enum AnalogInputChannel
    {
        CH_0,
        CH_1,
        CH_2,
        CH_3
    }

    public enum DigitalOutputPin : int
    {
        PIN_17 = 17,
        PIN_27 = 27,
        PIN_22 = 22,
        PIN_23 = 23,
        PIN_25 = 25,
        PIN_5 = 5,
        PIN_6 = 6,
        PIN_16 = 16
    }

    public enum AnalogOutputChannel
    {
        CH_0,
        CH_1
    }

    public class Peripherals
    {
        private readonly GpioController _gpioController;
        private readonly IAnalogInterface _analogInterface;

        public Peripherals(GpioController gpioController, IAnalogInterface analogInterface)
        {
            PwmChannels = new PwmChannel[] 
            {
                PwmChannel.Create(chip: 0, channel: 0),
                PwmChannel.Create(chip: 0, channel: 1)
            };
            foreach(var pwm in PwmChannels)
            {
                pwm.DutyCycle = 0;
                pwm.Start();
            }


            _analogInterface = analogInterface;
            _gpioController = gpioController;

            foreach (var pin in Enum.GetValues(typeof(DigitalOutputPin)))
            {
                _gpioController.OpenPin((int)pin, PinMode.Output);
            }

            foreach (var pin in Enum.GetValues(typeof(DigitalInputPin)))
            {
                _gpioController.OpenPin((int)pin, PinMode.Input);
            }
        }

        ~Peripherals()
        {
            foreach (var pwm in PwmChannels)
            {
                pwm.Dispose();
            }
        }

        public PwmChannel[] PwmChannels { get; }

        public void SetOutputPin(DigitalOutputPin pin, PinValue pinValue)
        {
            _gpioController.Write((int)pin, pinValue);
        }

        public PinValue ReadInputPin(DigitalInputPin pin)
        {
            return _gpioController.Read((int)pin);
        }

        public float ReadAnalogInput(AnalogInputChannel inputChannel)
        {
            return _analogInterface.ReadInput(inputChannel);
        }

        public void WriteToAnalogOutput(AnalogOutputChannel outputChannel, float value)
        {
            _analogInterface.WriteToOutput(outputChannel, value);
        }

    }
}
