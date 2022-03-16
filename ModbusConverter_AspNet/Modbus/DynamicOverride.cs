using EasyModbus;
using org.mariuszgromada.math.mxparser;
using System;
using System.Linq;

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
        private readonly Argument _argument = new(_argumentName);
        private readonly Expression _overrideExpression;

        public DynamicOverride()
        {
            _overrideExpression = new Expression(_argumentName, _argument);
        }

        public Guid Guid { get; } = Guid.NewGuid();
        public int Address { get; set; }
        public DataType DataType { get; set; }
        public string OverrideExpression
        {
            get => _overrideExpression.getExpressionString();
            set => _overrideExpression.setExpressionString(value);
        }

        public ushort[] Calculate(ushort[] data) => ApplyOverrideFuncToDataAsRegisters(data);

        public int LengthOfDataTypeInRegisters =>
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
                DataType.Int16 =>  BitConverter.ToInt16(BitConverter.GetBytes(data.FirstOrDefault())),
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
}
