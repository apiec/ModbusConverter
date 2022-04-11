using EasyModbus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModbusConverter.Modbus
{
    public class OverridingModbusServerWrapper : IModbusServerWrapper, IOverridesManager
    {
        private readonly ModbusServer _modbusServer;

        private readonly DataOverrider _coilOverrider;
        private readonly DataOverrider _discreteInputOverrider;
        private readonly DataOverrider _inputRegisterOverrider;
        private readonly DataOverrider _holdingRegisterOverrider;

        public IEnumerable<DynamicOverride> CoilsOverrides => _coilOverrider.Overrides;

        public IEnumerable<DynamicOverride> DiscreteInputsOverrides => _discreteInputOverrider.Overrides;

        public IEnumerable<DynamicOverride> HoldingRegistersOverrides => _holdingRegisterOverrider.Overrides;

        public IEnumerable<DynamicOverride> InputRegistersOverrides => _inputRegisterOverrider.Overrides;

        public OverridingModbusServerWrapper(ModbusServer modbusServer)
        {
            _modbusServer = modbusServer;
            _modbusServer.CoilsChanged += CoilsChangedHandler;
            _modbusServer.HoldingRegistersChanged += HoldingRegistersChangedHandler;

            _coilOverrider = new DataOverrider(
                (address, data) =>
                {
                    for (int i = 0; i < data.Length; ++i)
                    {
                        _modbusServer.coils[address + i] = Convert.ToBoolean(data[i]);
                    }
                },
                (address, length) =>
                {
                    var data = new ushort[length];
                    for (int i = 0; i < length; ++i)
                    {
                        data[i] = Convert.ToUInt16(_modbusServer.coils[address + i]);
                    }

                    return data;
                });

            _discreteInputOverrider = new DataOverrider(
                (address, data) =>
                {
                    for (int i = 0; i < data.Length; ++i)
                    {
                        _modbusServer.discreteInputs[address + i] = Convert.ToBoolean(data[i]);
                    }
                },
                (address, length) =>
                {
                    var data = new ushort[length];
                    for (int i = 0; i < length; ++i)
                    {
                        data[i] = Convert.ToUInt16(_modbusServer.discreteInputs[address + i]);
                    }

                    return data;
                });

            _inputRegisterOverrider = new DataOverrider(
                (address, data) =>
                {
                    for (int i = 0; i < data.Length; ++i)
                    {
                        _modbusServer.inputRegisters[address + i] = data[i];
                    }
                },
                (address, length) =>
                {
                    var data = new ushort[length];
                    for (int i = 0; i < length; ++i)
                    {
                        data[i] = _modbusServer.inputRegisters[address + i];
                    }

                    return data;
                });

            _holdingRegisterOverrider = new DataOverrider(
                (address, data) =>
                {
                    for (int i = 0; i < data.Length; ++i)
                    {
                        _modbusServer.holdingRegisters[address + i] = data[i];
                    }
                },
                (address, length) =>
                {
                    var data = new ushort[length];
                    for (int i = 0; i < length; ++i)
                    {
                        data[i] = _modbusServer.holdingRegisters[address + i];
                    }

                    return data;
                });
        }

        public event ModbusServer.CoilsChangedHandler CoilsChanged = delegate { };
        public event ModbusServer.HoldingRegistersChangedHandler HoldingRegistersChanged = delegate { };

        private void CoilsChangedHandler(int coil, int numberOfCoils)
        {
            var data = new ushort[numberOfCoils];
            for (int i = 0; i < numberOfCoils; ++i)
            {
                data[i] = Convert.ToUInt16(_modbusServer.coils[coil + i]);
            }

            _coilOverrider.Write(coil, data);

            CoilsChanged(coil, numberOfCoils);
        }

        private void HoldingRegistersChangedHandler(int register, int numberOfRegisters)
        {
            var data = new ushort[numberOfRegisters];
            for (int i = 0; i < numberOfRegisters; ++i)
            {
                data[i] = _modbusServer.holdingRegisters[register + i];
            }

            _holdingRegisterOverrider.Write(register, data);

            HoldingRegistersChanged(register, numberOfRegisters);
        }

        public void WriteToCoils(int address, bool[] registers)
        {
            var data = registers.Select(r => Convert.ToUInt16(r)).ToArray();

            _coilOverrider.Write(address, data);
        }

        public void WriteToDiscreteInputs(int address, bool[] registers)
        {
            var data = registers.Select(r => Convert.ToUInt16(r)).ToArray();

            _discreteInputOverrider.Write(address, data);
        }

        public void WriteToInputRegisters(int address, ushort[] registers)
        {
            _inputRegisterOverrider.Write(address, registers);
        }

        public void WriteToHoldingRegisters(int address, ushort[] registers)
        {
            _holdingRegisterOverrider.Write(address, registers);
        }

        public bool[] ReadCoils(int address, int numberOfRegisters)
        {
            var result = new bool[numberOfRegisters];
            for (int i = 0; i < numberOfRegisters; ++i)
            {
                result[i] = _modbusServer.coils[address + i];
            }
            return result;
        }

        public bool[] ReadDiscreteInputs(int address, int numberOfRegisters)
        {
            var result = new bool[numberOfRegisters];
            for (int i = 0; i < numberOfRegisters; ++i)
            {
                result[i] = _modbusServer.discreteInputs[address + i];
            }
            return result;
        }

        public ushort[] ReadInputRegisters(int address, int numberOfRegisters)
        {
            var result = new ushort[numberOfRegisters];
            for (int i = 0; i < numberOfRegisters; ++i)
            {
                result[i] = _modbusServer.inputRegisters[address + i];
            }
            return result;
        }

        public ushort[] ReadHoldingRegisters(int address, int numberOfRegisters)
        {
            var result = new ushort[numberOfRegisters];
            for (int i = 0; i < numberOfRegisters; ++i)
            {
                result[i] = _modbusServer.holdingRegisters[address + i];
            }
            return result;
        }

        public void AddCoilOverride(int address, string expression)
        {
            _coilOverrider.Override(address, expression, DataType.Bool);
        }

        public void AddDiscreteInputOverride(int address, string expression)
        {
            _discreteInputOverrider.Override(address, expression, DataType.Bool);
        }

        public void AddHoldingRegisterOverride(int address, string expression, DataType dataType)
        {
            _holdingRegisterOverrider.Override(address, expression, dataType);
        }

        public void AddInputRegisterOverride(int address, string expression, DataType dataType)
        {
            _inputRegisterOverrider.Override(address, expression, dataType);
        }

        public void RemoveCoilOverride(Guid guid)
        {
            _coilOverrider.StopOverriding(guid);
        }

        public void RemoveDiscreteInputOverride(Guid guid)
        {
            _discreteInputOverrider.StopOverriding(guid);
        }

        public void RemoveHoldingRegisterOverride(Guid guid)
        {
            _holdingRegisterOverrider.StopOverriding(guid);
        }

        public void RemoveInputRegisterOverride(Guid guid)
        {
            _inputRegisterOverrider.StopOverriding(guid);
        }
    }
}
