namespace ModbusConverter.PeripheralDevices
{
    public interface IAnalogInterface
    {
        void WriteToOutput(AnalogOutputChannel channel, float value);
        float ReadInput(AnalogInputChannel channel);
    }
}
