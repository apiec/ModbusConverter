using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyModbus;

namespace ModbusConverter.Modbus
{
    public class DataOverrider<TData>
    {
        private readonly Action<int, TData> _dataSetter;
        private readonly Func<int, TData> _dataGetter;
        private readonly Dictionary<int, TData> _overrides;

        public DataOverrider(Action<int, TData> dataSetter, Func<int, TData> dataGetter)
        {
            _dataSetter = dataSetter;
            _dataGetter = dataGetter;
        }

        public void Write(int address, TData data)
        {
            var dataToSet = _overrides.ContainsKey(address)
                ? _overrides[address]
                : data;

            _dataSetter(address, dataToSet);
        }
        
        public TData Read(int address)
        {
            return _overrides.ContainsKey(address)
                ? _overrides[address]
                : _dataGetter(address);
        }

        public void Override(int address, TData data)
        {
            _overrides[address] = data;
        }

        public void StopOverriding(int address)
        {
            _overrides.Remove(address);
        }

        public IEnumerable<KeyValuePair<int, TData>> Overrides => _overrides.AsEnumerable();
    }

    public class OverridingModbusServerWrapper : IModbusServerWrapper, IModbusOverrider
    {
        private readonly ModbusServer _modbusServer;

        private readonly DataOverrider<bool> _coilOverrider;
        private readonly DataOverrider<bool> _discreteInputOverrider;
        private readonly DataOverrider<ushort> _inputRegisterOverrider;
        private readonly DataOverrider<ushort> _holdingRegisterOverrider;

        public OverridingModbusServerWrapper(ModbusServer modbusServer)
        {
            _modbusServer = modbusServer;
            _modbusServer.CoilsChanged += CoilsChangedHandler;
            _modbusServer.HoldingRegistersChanged += HoldingRegistersChangedHandler;

            _coilOverrider = new DataOverrider<bool>(
                (address, value) => _modbusServer.coils[address] = value,
                address => _modbusServer.coils[address]);

            _discreteInputOverrider= new DataOverrider<bool>(
                (address, value) => _modbusServer.discreteInputs[address] = value,
                address => _modbusServer.discreteInputs[address]);

            _inputRegisterOverrider = new DataOverrider<ushort>(
                (address, value) => _modbusServer.inputRegisters[address] = value,
                address => _modbusServer.inputRegisters[address]);

            _holdingRegisterOverrider = new DataOverrider<ushort>(
                (address, value) => _modbusServer.holdingRegisters[address] = value,
                address => _modbusServer.holdingRegisters[address]);
        }

        public event ModbusServer.CoilsChangedHandler CoilsChanged;
        public event ModbusServer.HoldingRegistersChangedHandler HoldingRegistersChanged;

        private void CoilsChangedHandler(int coil, int numberOfCoils)
        {
            CoilsChanged(coil, numberOfCoils);
        }

        private void HoldingRegistersChangedHandler(int register, int numberOfRegisters)
        {
            HoldingRegistersChanged(register, numberOfRegisters);
        }

        public void WriteToCoils(int address, bool[] registers)
        {
            for (int i = 0; i < registers.Length; ++i)
            {
                _coilOverrider.Write(address + i, registers[i]);
            }
        }

        public void WriteToDiscreteInputs(int address, bool[] registers)
        {
            for (int i = 0; i < registers.Length; ++i)
            {
                _discreteInputOverrider.Write(address + i, registers[i]);
            }
        }

        public void WriteToInputRegisters(int address, ushort[] registers)
        {
            for (int i = 0; i < registers.Length; ++i)
            {
                _inputRegisterOverrider.Write(address + i, registers[i]);
            }
        }

        public void WriteToHoldingRegisters(int address, ushort[] registers)
        {
            for (int i = 0; i < registers.Length; ++i)
            {
                _holdingRegisterOverrider.Write(address + i, registers[i]);
            }
        }

        public bool[] ReadCoils(int address, int numberOfRegisters)
        {
            return Enumerable.Range(address, numberOfRegisters)
                .Select(x => _coilOverrider.Read(x))
                .ToArray();
        }

        public bool[] ReadDiscreteInputs(int address, int numberOfRegisters)
        {
            return Enumerable.Range(address, numberOfRegisters)
                .Select(x => _discreteInputOverrider.Read(x))
                .ToArray();
        }

        public ushort[] ReadInputRegisters(int address, int numberOfRegisters)
        {
            return Enumerable.Range(address, numberOfRegisters)
                .Select(x => _inputRegisterOverrider.Read(x))
                .ToArray();
        }

        public ushort[] ReadHoldingRegisters(int address, int numberOfRegisters)
        {
            return Enumerable.Range(address, numberOfRegisters)
                .Select(x => _holdingRegisterOverrider.Read(x))
                .ToArray();
        }

        public void OverrideCoils(IDictionary<int, bool> addressValuePairs)
        {
            foreach (var (address, value) in addressValuePairs)
            {
                _coilOverrider.Override(address, value);
            }
            CallCoilsChanged(addressValuePairs.Keys);
        }

        public void OverrideDiscreteInputs(IDictionary<int, bool> addressValuePairs)
        {
            foreach (var (address, value) in addressValuePairs)
            {
                _discreteInputOverrider.Override(address, value);
            }
        }

        public void OverrideInputRegisters(IDictionary<int, ushort> addressValuePairs)
        {
            foreach (var (address, value) in addressValuePairs)
            {
                _inputRegisterOverrider.Override(address, value);
            }
        }

        public void OverrideHoldingRegisters(IDictionary<int, ushort> addressValuePairs)
        {
            foreach (var (address, value) in addressValuePairs)
            {
                _holdingRegisterOverrider.Override(address, value);
            }
            CallHoldingRegistersChanged(addressValuePairs.Keys);
        }

        public void StopOverridingCoils(IEnumerable<int> addresses)
        {
            foreach (var address in addresses)
            {
                _coilOverrider.StopOverriding(address);
            }

            CallCoilsChanged(addresses);
        }

        public void StopOverridingDiscreteInputs(IEnumerable<int> addresses)
        {
            foreach (var address in addresses)
            {
                _discreteInputOverrider.StopOverriding(address);
            }
        }

        public void StopOverridingInputRegisters(IEnumerable<int> addresses)
        {
            foreach (var address in addresses)
            {
                _inputRegisterOverrider.StopOverriding(address);
            }
        }

        public void StopOverridingHoldingRegisters(IEnumerable<int> addresses)
        {
            foreach (var address in addresses)
            {
                _holdingRegisterOverrider.StopOverriding(address);
            }

            CallHoldingRegistersChanged(addresses);
        }

        public IEnumerable<KeyValuePair<int, bool>> GetCoilOverrides() => _coilOverrider.Overrides;

        public IEnumerable<KeyValuePair<int, bool>> GetDiscreteInputOverrides() => _discreteInputOverrider.Overrides;

        public IEnumerable<KeyValuePair<int, ushort>> GetInputRegisterOverrides() => _inputRegisterOverrider.Overrides;

        public IEnumerable<KeyValuePair<int, ushort>> GetHoldingRegisterOverrides() => _holdingRegisterOverrider.Overrides;

        private void CallCoilsChanged(IEnumerable<int> addresses)
        {
            var aggregated = AggregateAddresses(addresses);
            foreach (var (address, length) in aggregated)
            {
                CoilsChanged(address, length);
            }
        }

        private void CallHoldingRegistersChanged(IEnumerable<int> addresses)
        {
            var aggregated = AggregateAddresses(addresses);
            foreach (var (address, length) in aggregated)
            {
                HoldingRegistersChanged(address, length);
            }
        }

        private Dictionary<int, int> AggregateAddresses(IEnumerable<int> addresses)
        {
            if (addresses.Count() == 0)
                return default;

            var sorted = addresses
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            var addressesToLengths = new Dictionary<int, int>();

            int startOfCurrentSequence = sorted[0];
            addressesToLengths[startOfCurrentSequence] = 1;
            for (int i = 1; i < sorted.Length; ++i)
            {
                var subsequentAddressesDiff = sorted[i] - sorted[i - 1];
                if (subsequentAddressesDiff > 1)
                {
                    startOfCurrentSequence = sorted[i];
                    addressesToLengths[startOfCurrentSequence] = 1;
                }
                else
                {
                    addressesToLengths[startOfCurrentSequence] += 1;
                }
            }

            return addressesToLengths;
        }
    }
}
