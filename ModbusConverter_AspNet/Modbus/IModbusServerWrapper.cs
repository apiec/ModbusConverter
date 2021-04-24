using EasyModbus;
using System.Collections.Generic;

namespace ModbusConverter
{
    public interface IModbusServerWrapper
    {
        event ModbusServer.CoilsChangedHandler CoilsChanged;
        event ModbusServer.HoldingRegistersChangedHandler HoldingRegistersChanged;

        IEnumerable<KeyValuePair<int, bool>> GetCoilOverrides();
        IEnumerable<KeyValuePair<int, bool>> GetDiscreteInputOverrides();
        IEnumerable<KeyValuePair<int, ushort>> GetHoldingRegisterOverrides();
        IEnumerable<KeyValuePair<int, ushort>> GetInputRegisterOverrides();
        void OverrideCoils(IDictionary<int, bool> addressValuePairs);
        void OverrideDiscreteInputs(IDictionary<int, bool> addressValuePairs);
        void OverrideHoldingRegisters(IDictionary<int, ushort> addressValuePairs);
        void OverrideInputRegisters(IDictionary<int, ushort> addressValuePairs);
        bool[] ReadCoils(int address, int numberOfRegisters);
        bool[] ReadDiscreteInputs(int address, int numberOfRegisters);
        ushort[] ReadHoldingRegisters(int address, int numberOfRegisters);
        ushort[] ReadInputRegisters(int address, int numberOfRegisters);
        void StopOverridingCoils(IEnumerable<int> addresses);
        void StopOverridingDiscreteInputs(IEnumerable<int> addresses);
        void StopOverridingHoldingRegisters(IEnumerable<int> addresses);
        void StopOverridingInputRegisters(IEnumerable<int> addresses);
        void WriteToCoils(int address, bool[] registers);
        void WriteToDiscreteInputs(int address, bool[] registers);
        void WriteToHoldingRegisters(int address, ushort[] registers);
        void WriteToInputRegisters(int address, ushort[] registers);
    }
}