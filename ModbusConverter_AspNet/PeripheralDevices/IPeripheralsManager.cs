using ModbusConverter.PeripheralDevices.Peripherals;
using System.Collections.Generic;

namespace ModbusConverter.PeripheralDevices
{
    public interface IPeripheralsManager
    {
        IEnumerable<IPeripheral> Peripherals { get; }

        void AddPeripheral(IPeripheral peripheral);
        void AddPeripheralRange(IEnumerable<IPeripheral> peripherals);
        void LoadConfigFile();
        void RemovePeripheral(IPeripheral peripheral);
        void RemovePeripheralRange(IEnumerable<IPeripheral> peripherals);
        void SaveCurrentState();
    }
}