using ModbusConverter.PeripheralDevices.Peripherals;
using System.Collections.Generic;

namespace ModbusConverter.PeripheralDevices
{
    public interface IPeripheralsManager
    {
        IEnumerable<IInputPeripheral> InputPeripherals { get; }
        IEnumerable<IOutputPeripheral> OutputPeripherals { get; }
        IEnumerable<IPeripheral> Peripherals { get; }

        void AddPeripheral(IPeripheral peripheral);
        void RemovePeripheral(IPeripheral peripheral);
    }
}