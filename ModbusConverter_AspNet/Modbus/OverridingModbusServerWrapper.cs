using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyModbus;

namespace ModbusConverter.Modbus
{
    public class OverridingModbusServerWrapper : IModbusServerWrapper, IModbusOverrider
    {
        private readonly ModbusServer _modbusServer;
        private readonly Dictionary<int, bool> _coilOverrides = new Dictionary<int, bool>();
        private readonly Dictionary<int, bool> _discreteInputOverrides = new Dictionary<int, bool>();
        private readonly Dictionary<int, ushort> _inputRegisterOverrides = new Dictionary<int, ushort>();
        private readonly Dictionary<int, ushort> _holdingRegisterOverrides = new Dictionary<int, ushort>();

        public OverridingModbusServerWrapper(ModbusServer modbusServer)
        {
            _modbusServer = modbusServer;
            _modbusServer.CoilsChanged += CoilsChangedHandler;
            _modbusServer.HoldingRegistersChanged += HoldingRegistersChangedHandler;
        }

        #region Events
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

        #endregion

        #region WriteTo
        public void WriteToCoils(int address, bool[] registers)
        {
            for (int i = 0; i < registers.Length; ++i)
            {
                var currentCoil = address + i;
                var valueToWrite = _coilOverrides.ContainsKey(currentCoil)
                    ? _coilOverrides[currentCoil]
                    : registers[i];

                _modbusServer.coils[currentCoil] = valueToWrite;
            }
        }

        public void WriteToDiscreteInputs(int address, bool[] registers)
        {
            for (int i = 0; i < registers.Length; ++i)
            {
                var currentInput = address + i;
                var valueToWrite = _discreteInputOverrides.ContainsKey(currentInput)
                    ? _discreteInputOverrides[currentInput]
                    : registers[i];

                _modbusServer.discreteInputs[currentInput] = valueToWrite;
            }
        }

        public void WriteToInputRegisters(int address, ushort[] registers)
        {
            for (int i = 0; i < registers.Length; ++i)
            {
                var currentRegister = address + i;
                var valueToWrite = _inputRegisterOverrides.ContainsKey(currentRegister)
                    ? _inputRegisterOverrides[currentRegister]
                    : registers[i];

                _modbusServer.inputRegisters[currentRegister] = valueToWrite;
            }
        }

        public void WriteToHoldingRegisters(int address, ushort[] registers)
        {
            for (int i = 0; i < registers.Length; ++i)
            {
                var currentRegister = address + i;
                var valueToWrite = _holdingRegisterOverrides.ContainsKey(currentRegister)
                    ? _holdingRegisterOverrides[currentRegister]
                    : registers[i];

                _modbusServer.holdingRegisters[currentRegister] = valueToWrite;
            }
        }
        #endregion

        #region Read
        public bool[] ReadCoils(int address, int numberOfRegisters)
        {
            var toReturn = new bool[numberOfRegisters];
            for (int i = 0; i < numberOfRegisters; ++i)
            {
                var currentCoil = address + i;
                var valueToWrite = _coilOverrides.ContainsKey(currentCoil)
                    ? _coilOverrides[currentCoil]
                    : _modbusServer.coils[currentCoil];

                toReturn[i] = valueToWrite;
            }

            return toReturn;
        }

        public bool[] ReadDiscreteInputs(int address, int numberOfRegisters)
        {
            var toReturn = new bool[numberOfRegisters];

            for (int i = 0; i < numberOfRegisters; ++i)
            {
                var currentInput = address + i;
                var valueToWrite = _discreteInputOverrides.ContainsKey(currentInput)
                    ? _discreteInputOverrides[currentInput]
                    : _modbusServer.discreteInputs[currentInput];

                toReturn[i] = valueToWrite;
            }

            return toReturn;
        }

        public ushort[] ReadInputRegisters(int address, int numberOfRegisters)
        {
            var toReturn = new ushort[numberOfRegisters];

            for (int i = 0; i < numberOfRegisters; ++i)
            {
                var currentRegister = address + i;
                var valueToWrite = _inputRegisterOverrides.ContainsKey(currentRegister)
                    ? _inputRegisterOverrides[currentRegister]
                    : _modbusServer.inputRegisters[currentRegister];

                toReturn[i] = valueToWrite;
            }

            return toReturn;
        }

        public ushort[] ReadHoldingRegisters(int address, int numberOfRegisters)
        {
            var toReturn = new ushort[numberOfRegisters];

            for (int i = 0; i < numberOfRegisters; ++i)
            {
                var currentRegister = address + i;
                var valueToWrite = _holdingRegisterOverrides.ContainsKey(currentRegister)
                    ? _holdingRegisterOverrides[currentRegister]
                    : _modbusServer.holdingRegisters[currentRegister];

                toReturn[i] = valueToWrite;
            }

            return toReturn;
        }
        #endregion

        #region Overrides

        public void OverrideCoils(IDictionary<int, bool> addressValuePairs)
        {
            foreach (var (address, value) in addressValuePairs)
            {
                _coilOverrides[address] = value;
            }
            CallCoilsChanged(addressValuePairs.Keys);
        }

        public void OverrideDiscreteInputs(IDictionary<int, bool> addressValuePairs)
        {
            foreach (var (address, value) in addressValuePairs)
            {
                _discreteInputOverrides[address] = value;
            }
        }

        public void OverrideInputRegisters(IDictionary<int, ushort> addressValuePairs)
        {
            foreach (var (address, value) in addressValuePairs)
            {
                _inputRegisterOverrides[address] = value;
            }
        }

        public void OverrideHoldingRegisters(IDictionary<int, ushort> addressValuePairs)
        {
            foreach (var (address, value) in addressValuePairs)
            {
                _holdingRegisterOverrides[address] = value;
            }

            CallHoldingRegistersChanged(addressValuePairs.Keys);
        }

        public void StopOverridingCoils(IEnumerable<int> addresses)
        {
            foreach (var address in addresses)
            {
                _coilOverrides.Remove(address);
            }

            CallCoilsChanged(addresses);
        }

        public void StopOverridingDiscreteInputs(IEnumerable<int> addresses)
        {
            foreach (var address in addresses)
            {
                _discreteInputOverrides.Remove(address);
            }
        }

        public void StopOverridingInputRegisters(IEnumerable<int> addresses)
        {
            foreach (var address in addresses)
            {
                _inputRegisterOverrides.Remove(address);
            }
        }

        public void StopOverridingHoldingRegisters(IEnumerable<int> addresses)
        {
            foreach (var address in addresses)
            {
                _holdingRegisterOverrides.Remove(address);
            }

            CallHoldingRegistersChanged(addresses);
        }

        public IEnumerable<KeyValuePair<int, bool>> GetCoilOverrides()
        {
            return _coilOverrides.AsEnumerable();
        }

        public IEnumerable<KeyValuePair<int, bool>> GetDiscreteInputOverrides()
        {
            return _discreteInputOverrides.AsEnumerable();
        }

        public IEnumerable<KeyValuePair<int, ushort>> GetInputRegisterOverrides()
        {
            return _inputRegisterOverrides.AsEnumerable();
        }

        public IEnumerable<KeyValuePair<int, ushort>> GetHoldingRegisterOverrides()
        {
            return _holdingRegisterOverrides.AsEnumerable();
        }

        #endregion

        #region Utils
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

            var sorted = addresses.OrderBy(i => i).ToArray();

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
        #endregion

    }
}
