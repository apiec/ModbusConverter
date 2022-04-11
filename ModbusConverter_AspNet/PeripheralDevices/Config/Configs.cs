using ModbusConverter.PeripheralDevices.Peripherals;

namespace ModbusConverter.PeripheralDevices.Config
{
    public class PeripheralConfig
    {
        public PeripheralConfig()
        {
        }

        public PeripheralConfig(IPeripheral peripheral)
        {
            RegisterType = peripheral.RegisterType.ToString();
            RegisterAddress = peripheral.RegisterAddress;
            Name = peripheral.Name;
            Type = peripheral.GetType().Name;
        }

        public string Type { get; set; }
        public string RegisterType { get; set; }
        public int RegisterAddress { get; set; }
        public string Name { get; set; }
    }

    public class PwmPinConfig : PeripheralConfig
    {
        public PwmPinConfig()
        {
        }

        public PwmPinConfig(PwmPin pwmPin)
            : base(pwmPin)
        {
            PinNumber = pwmPin.PinNumber;
        }
        public int PinNumber { get; set; }
    }

    public class InputPinConfig : PeripheralConfig
    {
        public InputPinConfig()
        {
        }

        public InputPinConfig(InputPin inputPin)
            : base(inputPin)
        {
            PinNumber = inputPin.PinNumber;
        }
        public int PinNumber { get; set; }
    }

    public class OutputPinConfig : PeripheralConfig
    {
        public OutputPinConfig()
        {
        }

        public OutputPinConfig(OutputPin outputPin)
            : base(outputPin)
        {
            PinNumber = outputPin.PinNumber;
        }
        public int PinNumber { get; set; }
    }

    public class AnalogInputChannelConfig : PeripheralConfig
    {
        public AnalogInputChannelConfig()
        {
        }

        public AnalogInputChannelConfig(AnalogInputChannel analogInputChannel)
            : base(analogInputChannel)
        {
            PCF8591Number = analogInputChannel.PCF8591Number;
            InputMode = analogInputChannel.InputMode.ToString();
        }
        public int PCF8591Number { get; set; }
        public string InputMode { get; set; }
    }

    public class AnalogOutputChannelConfig : PeripheralConfig
    {
        public AnalogOutputChannelConfig()
        {
        }

        public AnalogOutputChannelConfig(AnalogOutputChannel analogOutputChannel)
            : base(analogOutputChannel)
        {
            PCF8591Number = analogOutputChannel.PCF8591Number;
        }
        public int PCF8591Number { get; set; }
    }
}
