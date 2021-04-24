namespace ModbusConverter.PeripheralDevices.AnalogIO
{
    public interface IPCF8591DeviceFactory
    {
        PCF8591Device CreatePCF8591Device(int address);
    }
}