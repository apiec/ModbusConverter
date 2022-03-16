using Xunit;
using FluentAssertions;
using Moq;
using ModbusConverter.Modbus;
using System;
using EasyModbus;

namespace ModbusConverterTests.Modbus
{
    public class DynamicOverrideTests
    {
        public DynamicOverrideTests()
        {
        }

        [Theory]
        [InlineData(DataType.Bool, 1)]
        [InlineData(DataType.UInt16, 1)]
        [InlineData(DataType.Int16, 1)]
        [InlineData(DataType.Int32, 2)]
        [InlineData(DataType.Int64, 4)]
        [InlineData(DataType.Float, 2)]
        [InlineData(DataType.Double, 4)]
        public void LengthOfDataTypeInRegisters_ShouldReturnCorrectValue(
            DataType dataType, int length)
        {
            //Arrange
            var dynamicOverride = new DynamicOverride();
            dynamicOverride.DataType = dataType;

            //Act
            //Assert
            dynamicOverride.LengthOfDataTypeInRegisters
                .Should().Be(length);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(150)]
        [InlineData(40000)]
        public void Address_ShouldBeSetProperly(int address)
        {
            //Arrange
            var dynamicOverride = new DynamicOverride();
            dynamicOverride.Address = address;

            //Act
            //Assert
            dynamicOverride.Address
                .Should().Be(address);
        }

        [Theory]
        [InlineData("1", true, true)]
        [InlineData("1", false, true)]
        [InlineData("0", true, false)]
        [InlineData("0", false, false)]
        [InlineData("x", true, true)]
        [InlineData("x", false, false)]
        [InlineData("~x", true, false)]
        [InlineData("~x", false, true)]
        public void Calculate_DataTypeIsBool_ShouldGiveProperResults(
            string expression, bool input, bool expectedOutput)
        {
            //Arrange
            var dataType = DataType.Bool;
            var dynamicOverride = new DynamicOverride();
            dynamicOverride.DataType = dataType;
            dynamicOverride.OverrideExpression = expression;

            var inputRegisters = ConvertDataToRegisters(dataType, input);
            var expectedOutputRegisters = ConvertDataToRegisters(dataType, expectedOutput);

            //Act
            var outputRegisters = dynamicOverride.Calculate(inputRegisters);

            //Assert
            outputRegisters
                .Should()
                .BeEquivalentTo(expectedOutputRegisters);
        }

        [Theory]
        [InlineData("1", 10, 1)]
        [InlineData("0", 10, 0)]
        [InlineData("x", 10, 10)]
        [InlineData("x", 0, 0)]
        [InlineData("x+10", 1, 11)]
        [InlineData("x-10", 65535, 65525)]
        [InlineData("x*10", 10, 100)]
        [InlineData("x/10", 100, 10)]
        [InlineData("x/10", 105, 10)]
        [InlineData("x^2", 10, 100)]
        [InlineData("10+x^2", 2, 14)]
        public void Calculate_DataTypeIsUint16_ShouldGiveProperResults(
            string expression, ushort input, ushort expectedOutput)
        {
            //Arrange
            var dataType = DataType.UInt16;
            var dynamicOverride = new DynamicOverride();
            dynamicOverride.DataType = dataType;
            dynamicOverride.OverrideExpression = expression;

            var inputRegisters = ConvertDataToRegisters(dataType, input);
            var expectedOutputRegisters = ConvertDataToRegisters(dataType, expectedOutput);

            //Act
            var outputRegisters = dynamicOverride.Calculate(inputRegisters);

            //Assert
            outputRegisters
                .Should()
                .BeEquivalentTo(expectedOutputRegisters);
        }

        [Theory]
        [InlineData("1", 10, 1)]
        [InlineData("0", 10, 0)]
        [InlineData("x", 10, 10)]
        [InlineData("x", 0, 0)]
        [InlineData("x-10", 1, -9)]
        [InlineData("x*10", 10, 100)]
        [InlineData("x/10", 100, 10)]
        [InlineData("x/10", 105, 10)]
        [InlineData("x^2", 10, 100)]
        [InlineData("x^2", -1, 1)]
        [InlineData("10+x^2", 2, 14)]
        public void Calculate_DataTypeIsInt16_ShouldGiveProperResults(
            string expression, short input, short expectedOutput)
        {
            //Arrange
            var dataType = DataType.Int16;
            var dynamicOverride = new DynamicOverride();
            dynamicOverride.DataType = dataType;
            dynamicOverride.OverrideExpression = expression;

            var inputRegisters = ConvertDataToRegisters(dataType, input);
            var expectedOutputRegisters = ConvertDataToRegisters(dataType, expectedOutput);

            //Act
            var outputRegisters = dynamicOverride.Calculate(inputRegisters);

            //Assert
            outputRegisters
                .Should()
                .BeEquivalentTo(expectedOutputRegisters);
        }



        private ushort[] ConvertDataToRegisters(DataType dataType, object data) =>
            dataType switch
            {
                DataType.Bool => new[] { Convert.ToUInt16((bool)data) },
                DataType.UInt16 => new[] { (ushort) data },
                DataType.Int16 => new[] { BitConverter.ToUInt16(BitConverter.GetBytes((short)data)) },
                DataType.Int32 => ModbusClient.ConvertIntToRegisters((int)data),
                DataType.Int64 => ModbusClient.ConvertLongToRegisters((long)data),
                DataType.Float => ModbusClient.ConvertFloatToRegisters((float)data),
                DataType.Double => ModbusClient.ConvertDoubleToRegisters((double)data),
                _ => throw new NotImplementedException()
            };
}
}
