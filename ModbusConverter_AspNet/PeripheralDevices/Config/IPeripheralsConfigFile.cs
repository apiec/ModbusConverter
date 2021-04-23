using ModbusConverter.PeripheralDevices.Peripherals;
using System.Collections.Generic;

namespace ModbusConverter.PeripheralDevices.Config
{
    public interface IPeripheralsConfigFile
    {
        IEnumerable<IPeripheral> ReadConfigFile();
        void WriteToConfigFile(IEnumerable<IPeripheral> peripherals);
    }
}