namespace ModbusConverter.PeripheralDevices
{
    public interface IOutputsUpdater
    {
        void OnCoilsChanged(int coil, int numberOfCoils);
        void OnHoldingRegistersChanged(int register, int numberOfRegisters);
    }
}