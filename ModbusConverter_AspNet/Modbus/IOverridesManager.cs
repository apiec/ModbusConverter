using System;
using System.Collections.Generic;

namespace ModbusConverter.Modbus
{
    public interface IOverridesManager
    {
        void AddCoilOverride(int address, string expression);
        void AddDiscreteInputOverride(int address, string expression);
        void AddHoldingRegisterOverride(int address, string expression, DataType dataType);
        void AddInputRegisterOverride(int address, string expression, DataType dataType);

        void RemoveCoilOverride(Guid guid);
        void RemoveDiscreteInputOverride(Guid guid);
        void RemoveHoldingRegisterOverride(Guid guid);
        void RemoveInputRegisterOverride(Guid guid);

        IEnumerable<DynamicOverride> CoilsOverrides { get; }
        IEnumerable<DynamicOverride> DiscreteInputsOverrides { get; }
        IEnumerable<DynamicOverride> HoldingRegistersOverrides { get; }
        IEnumerable<DynamicOverride> InputRegistersOverrides { get; }

    }
}