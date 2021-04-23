using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.PeripheralDevices.Peripherals;
using ModbusConverter.PeripheralDevices.Config;

namespace ModbusConverter.PeripheralDevices
{
    public class PeripheralsManager : IPeripheralsManager
    {
        private HashSet<IPeripheral> _peripherals;
        private readonly IPeripheralsConfigFile _peripheralsConfigFile;

        public PeripheralsManager(IPeripheralsConfigFile peripheralsConfigFile)
        {
            _peripheralsConfigFile = peripheralsConfigFile;
            _peripherals = peripheralsConfigFile
                .ReadConfigFile()
                .ToHashSet();
        }

        public IEnumerable<IPeripheral> Peripherals => _peripherals.AsEnumerable();
        public IEnumerable<IOutputPeripheral> OutputPeripherals
            => _peripherals
                .Where(peripheral => peripheral is IOutputPeripheral)
                .Select(peripheral => (IOutputPeripheral)peripheral);

        public IEnumerable<IInputPeripheral> InputPeripherals
            => _peripherals
                .Where(peripheral => peripheral is IInputPeripheral)
                .Select(peripheral => (IInputPeripheral)peripheral);

        public void AddPeripheralRange(IEnumerable<IPeripheral> peripherals)
        {
            foreach (var peripheral in peripherals)
                AddPeripheral(peripheral);
        }

        public void RemovePeripheralRange(IEnumerable<IPeripheral> peripherals)
        {
            foreach (var peripheral in peripherals)
                RemovePeripheral(peripheral);
        }
        public void AddPeripheral(IPeripheral peripheral)
        {
            if (peripheral is null)
            {
                throw new ArgumentNullException();
            }

            _peripherals.Add(peripheral);
        }

        public void RemovePeripheral(IPeripheral peripheral)
        {
            if (peripheral is null)
            {
                throw new ArgumentNullException();
            }

            _peripherals.Remove(peripheral);
        }

        public void SaveCurrentState()
        {
            _peripheralsConfigFile.WriteToConfigFile(Peripherals);
        }

        public void ReloadConfigFile()
        {
            _peripherals = _peripheralsConfigFile
                .ReadConfigFile()
                .ToHashSet();
        }

    }
}
