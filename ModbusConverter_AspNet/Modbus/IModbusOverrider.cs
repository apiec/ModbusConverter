using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModbusConverter.Modbus
{
    public interface IModbusOverrider
    {
        IEnumerable<KeyValuePair<int, bool>> GetCoilOverrides();
        IEnumerable<KeyValuePair<int, bool>> GetDiscreteInputOverrides();
        IEnumerable<KeyValuePair<int, ushort>> GetHoldingRegisterOverrides();
        IEnumerable<KeyValuePair<int, ushort>> GetInputRegisterOverrides();

        void OverrideCoils(IDictionary<int, bool> addressValuePairs);
        void OverrideDiscreteInputs(IDictionary<int, bool> addressValuePairs);
        void OverrideHoldingRegisters(IDictionary<int, ushort> addressValuePairs);
        void OverrideInputRegisters(IDictionary<int, ushort> addressValuePairs);

        void StopOverridingCoils(IEnumerable<int> addresses);
        void StopOverridingDiscreteInputs(IEnumerable<int> addresses);
        void StopOverridingHoldingRegisters(IEnumerable<int> addresses);
        void StopOverridingInputRegisters(IEnumerable<int> addresses);
    }
}
