namespace ModbusConverter.PeripheralDevices.AnalogIO
{
    public interface IAnalogIOController
    {
        byte ReadInput(int pcf8591Number, PCF8591Device.InputMode inputMode);
        void WriteToOutput(int pcf8591Number, byte value);
    }
}
