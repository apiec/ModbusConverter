using ModbusConverter.Modbus;
using ModbusConverter.PeripheralDevices.Config;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public interface IPeripheral
    {
        ModbusRegisterType RegisterType { get; set; }
        int RegisterAddress { get; set; }
        string Name { get; set; }

        void Update();
        PeripheralConfig GetConfig();
    }

}
