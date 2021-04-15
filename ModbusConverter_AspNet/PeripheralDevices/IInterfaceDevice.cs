namespace ModbusConverter.PeripheralDevices
{
    public interface IInterfaceDevice
    {
        byte ReadInput(int inputChannel);
        void WriteToOutput(int outputChannel, byte value);
    }
}