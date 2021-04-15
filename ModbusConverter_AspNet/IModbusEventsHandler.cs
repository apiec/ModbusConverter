namespace ModbusConverter
{
    public interface IModbusEventsHandler
    {
        void CoilsChangedHandler(int coil, int numberOfCoils);
        void HoldingRegistersChangedHandler(int register, int numberOfRegisters);
    }
}