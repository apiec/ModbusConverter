using ModbusConverter.PeripheralDevices.Peripherals;
using System.Collections.Generic;

namespace ModbusConverter.PeripheralDevices.Config
{
    public interface IPeripheralsConfigFile
    {
        IEnumerable<IPeripheral> ReadConfigFile();
        string SerializePeripherals(IEnumerable<IPeripheral> peripherals);
        void WriteToConfigFile(IEnumerable<IPeripheral> peripherals);
    }
}