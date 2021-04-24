using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyModbus;
using ModbusConverter;

namespace WebAppSandbox
{
    public class MockModbusServerWrapper : IModbusServerWrapper
    {
        private readonly Dictionary<int, bool> _coilOverrides = new Dictionary<int, bool>();
        private readonly Dictionary<int, bool> _discreteInputOverrides = new Dictionary<int, bool>();
        private readonly Dictionary<int, ushort> _inputRegisterOverrides = new Dictionary<int, ushort>();
        private readonly Dictionary<int, ushort> _holdingRegisterOverrides = new Dictionary<int, ushort>();


        public event ModbusServer.CoilsChangedHandler CoilsChanged;
        public event ModbusServer.HoldingRegistersChangedHandler HoldingRegistersChanged;
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
        public bool[] ReadCoils(int address, int numberOfRegisters)
        {
            throw new NotImplementedException();
        }

        public bool[] ReadDiscreteInputs(int address, int numberOfRegisters)
        {
            throw new NotImplementedException();
        }

        public ushort[] ReadHoldingRegisters(int address, int numberOfRegisters)
        {
            throw new NotImplementedException();
        }

        public ushort[] ReadInputRegisters(int address, int numberOfRegisters)
        {
            throw new NotImplementedException();
        }


        public void OverrideCoils(IDictionary<int, bool> addressValuePairs)
        {
            foreach (var (address, value) in addressValuePairs)
            {
                _coilOverrides[address] = value;
            }
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

        }

        public void StopOverridingCoils(IEnumerable<int> addresses)
        {
            foreach (var address in addresses)
            {
                _coilOverrides.Remove(address);
            }

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

        }

        public void WriteToCoils(int address, bool[] registers)
        {
            throw new NotImplementedException();
        }

        public void WriteToDiscreteInputs(int address, bool[] registers)
        {
            throw new NotImplementedException();
        }

        public void WriteToHoldingRegisters(int address, ushort[] registers)
        {
            throw new NotImplementedException();
        }

        public void WriteToInputRegisters(int address, ushort[] registers)
        {
            throw new NotImplementedException();
        }
    }
}
