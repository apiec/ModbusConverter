using ModbusConverter.PeripheralDevices.Config;
using ModbusConverter.PeripheralDevices.Peripherals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModbusConverter.PeripheralDevices
{
    public class PeripheralsManager : IPeripheralsManager
    {
        private HashSet<IPeripheral> _peripherals;
        private readonly IPeripheralsConfigFile _peripheralsConfigFile;

        public PeripheralsManager(IPeripheralsConfigFile peripheralsConfigFile)
        {
            _peripheralsConfigFile = peripheralsConfigFile;
            LoadConfigFile();
        }

        public IEnumerable<IPeripheral> Peripherals => _peripherals.AsEnumerable();

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

        public void LoadConfigFile()
        {
            _peripherals = _peripheralsConfigFile
                .ReadConfigFile()
                .ToHashSet();
        }

    }
}
