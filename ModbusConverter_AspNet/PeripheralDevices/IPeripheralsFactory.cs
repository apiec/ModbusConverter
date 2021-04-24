using ModbusConverter.PeripheralDevices.AnalogIO;
using ModbusConverter.PeripheralDevices.Config;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public interface IPeripheralsFactory
    {
        AnalogInputChannel CreateAnalogInputChannel(int pcf8591Number, PCF8591Device.InputMode inputMode);
        AnalogOutputChannel CreateAnalogOutputChannel(int pcf8591Number);
        IPeripheral CreateFromConfig(PeripheralConfig peripheralConfig);
        InputPin CreateInputPin(int pinNumber);
        OutputPin CreateOutputPin(int pinNumber);
        PwmPin CreatePwmPin(int pinNumber);
    }
}