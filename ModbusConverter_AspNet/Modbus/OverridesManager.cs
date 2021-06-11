using System;
using System.Collections.Generic;
using System.Linq;
using EasyModbus;
using org.mariuszgromada.math.mxparser;

namespace ModbusConverter.Modbus
{
    public enum DataType
    {
        Bool,
        UInt16,
        Int16,
        Int32,
        Int64,
        Float,
        Double
    }

    public class DynamicOverride
    {
        private static readonly string _argumentName = "x";

        private readonly Action<int, ushort[]> _dataSetter;
        private readonly Func<int, int, ushort[]> _dataGetter;

        private Expression _overrideExpression;

        public DynamicOverride(Action<int, ushort[]> dataSetter, Func<int, int, ushort[]> dataGetter)
        {
            _dataSetter = dataSetter;
            _dataGetter = dataGetter;
        }

        public int Address { get; set; }
        public DataType DataType { get; set; }
        public string OverrideExpression
        {
            get => _overrideExpression.getExpressionString();
            set => _overrideExpression = new Expression(value);
        }

        public void UpdateData()
        {
            var data = _dataGetter(Address, LengthOfDataTypeInRegisters);
            var overridenData = ApplyOverrideFuncToDataAsRegisters(data);
            _dataSetter(Address, overridenData);
        }

        private int LengthOfDataTypeInRegisters =>
            DataType switch
            {
                DataType.Bool => 1,
                DataType.UInt16 => 1,
                DataType.Int16 => 1,
                DataType.Int32 => 2,
                DataType.Int64 => 4,
                DataType.Float => 2,
                DataType.Double => 4,
                _ => throw new NotImplementedException()
            };

        private ushort[] ApplyOverrideFuncToDataAsRegisters(ushort[] data)
        {
            if (_overrideExpression is null)
                return data;

            var dataAsDouble = ReadDataAndConvertToDouble(data);
            var overridenData = CalculateOverridenValue(dataAsDouble);
            var overridenDataAsRegisters = ConvertDataToRegisters(overridenData);

            return overridenDataAsRegisters;
        }

        private double ReadDataAndConvertToDouble(ushort[] data) =>
            DataType switch
            {
                DataType.Bool => data.FirstOrDefault(),
                DataType.UInt16 => data.FirstOrDefault(),
                DataType.Int16 => data.FirstOrDefault(),
                DataType.Int32 => ModbusClient.ConvertRegistersToInt(data),
                DataType.Int64 => ModbusClient.ConvertRegistersToLong(data),
                DataType.Float => ModbusClient.ConvertRegistersToFloat(data),
                DataType.Double => ModbusClient.ConvertRegistersToDouble(data),
                _ => throw new NotImplementedException()
            };

        private double CalculateOverridenValue(double data)
        {
            _overrideExpression.setArgumentValue(_argumentName, data);
            return _overrideExpression.calculate();
        }

        private ushort[] ConvertDataToRegisters(double data) =>
            DataType switch
            {
                DataType.Bool => new[] { Convert.ToUInt16(Convert.ToBoolean(data)) },
                DataType.UInt16 => new[] { Convert.ToUInt16(data) },
                DataType.Int16 => new[] { BitConverter.ToUInt16(BitConverter.GetBytes(Convert.ToInt16(data))) },
                DataType.Int32 => ModbusClient.ConvertIntToRegisters(Convert.ToInt32(data)),
                DataType.Int64 => ModbusClient.ConvertLongToRegisters(Convert.ToInt64(data)),
                DataType.Float => ModbusClient.ConvertFloatToRegisters((float)data),
                DataType.Double => ModbusClient.ConvertDoubleToRegisters(data),
                _ => throw new NotImplementedException()
            };

    }

    public class OverridesManager
    {
        private readonly HashSet<DynamicOverride> _coilOverrides = new();
        private readonly HashSet<DynamicOverride> _discreteInputOverrides = new();
        private readonly HashSet<DynamicOverride> _inputRegistersOverrides = new();
        private readonly HashSet<DynamicOverride> _holdingRegistersOverrides = new();
        private readonly IModbusServerWrapper _modbusServerWrapper;

        public OverridesManager(IModbusServerWrapper modbusServerWrapper)
        {
            _modbusServerWrapper = modbusServerWrapper;
        }

        public void AddCoilOverride(string expression)
        {
            var @override = GetNewCoilOverride();
            @override.OverrideExpression = expression;
            @override.DataType = DataType.Bool;
            _coilOverrides.Add(@override);
        }

        public void AddDiscreteInputOverride(string expression)
        {
            var @override = GetNewDiscreteInputOverride();
            @override.OverrideExpression = expression;
            @override.DataType = DataType.Bool;
            _discreteInputOverrides.Add(@override);
        }

        public void AddInputRegisterOverride(string expression, DataType dataType)
        {
            var @override = GetNewInputRegisterOverride();
            @override.OverrideExpression = expression;
            @override.DataType = dataType;
            _inputRegistersOverrides.Add(@override);
        }

        public void AddHoldingRegisterOverride(string expression, DataType dataType)
        {
            var @override = GetNewHoldingRegisterOverride();
            @override.OverrideExpression = expression;
            @override.DataType = dataType;
            _holdingRegistersOverrides.Add(@override);
        }

        private DynamicOverride GetNewCoilOverride() => new DynamicOverride(CoilsSetter, CoilsGetter);
        private DynamicOverride GetNewDiscreteInputOverride() => new DynamicOverride(DiscreteInputsSetter, DiscreteInputsGetter);
        private DynamicOverride GetNewInputRegisterOverride() => new DynamicOverride(InputRegistersSetter, InputRegistersGetter);
        private DynamicOverride GetNewHoldingRegisterOverride() => new DynamicOverride(HoldingRegistersSetter, HoldingRegistersGetter);

        private void CoilsSetter(int address, ushort[] data)
        {
            var boolData = data
                .Select(x => Convert.ToBoolean(x))
                .ToArray();

            _modbusServerWrapper.WriteToCoils(address, boolData);
        }

        private void DiscreteInputsSetter(int address, ushort[] data)
        {
            var boolData = data
                .Select(x => Convert.ToBoolean(x))
                .ToArray();

            _modbusServerWrapper.WriteToDiscreteInputs(address, boolData);
        }

        private void InputRegistersSetter(int address, ushort[] data) => _modbusServerWrapper.WriteToInputRegisters(address, data);
        private void HoldingRegistersSetter(int address, ushort[] data) => _modbusServerWrapper.WriteToHoldingRegisters(address, data);

        private ushort[] CoilsGetter(int address, int count)
        {
            var boolData = _modbusServerWrapper.ReadCoils(address, count);

            return boolData
                .Select(x => Convert.ToUInt16(x))
                .ToArray();
        }

        private ushort[] DiscreteInputsGetter(int address, int count)
        {
            var boolData = _modbusServerWrapper.ReadDiscreteInputs(address, count);

            return boolData
                .Select(x => Convert.ToUInt16(x))
                .ToArray();
        }

        private ushort[] InputRegistersGetter(int address, int count) => _modbusServerWrapper.ReadInputRegisters(address, count);
        private ushort[] HoldingRegistersGetter(int address, int count) => _modbusServerWrapper.ReadHoldingRegisters(address, count);

    }
}
