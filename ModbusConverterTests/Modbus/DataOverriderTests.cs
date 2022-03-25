using Xunit;
using FluentAssertions;
using Moq;
using ModbusConverter.Modbus;
using System;
using System.Linq;

namespace ModbusConverterTests.Modbus
{
    public class DataOverriderTests
    {

        Mock<Action<int, ushort[]>> _dataSetterMock = new();
        Mock<Func<int, int, ushort[]>> _dataGetterMock = new();
        DataOverrider _dataOverrider;

        public DataOverriderTests()
        {
            _dataOverrider = new DataOverrider(_dataSetterMock.Object, _dataGetterMock.Object);
        }

        [Fact]
        public void Override_ShouldCreateANewOverride()
        {
            //Arrange
            const int address = 0;
            const string expression = "x";
            const DataType dataType = DataType.UInt16;

            _dataGetterMock
                .Setup(getter => getter.Invoke(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ushort[] { 0 });

            //Act
            _dataOverrider.Override(address, expression, dataType);

            //Assert
            _dataOverrider.Overrides
                .Should().HaveCount(1);

            var ovrd = _dataOverrider.Overrides.First();
            ovrd.Address.Should().Be(address);
            ovrd.OverrideExpression.Should().Be(expression);
            ovrd.DataType.Should().Be(dataType);
        }

        [Fact]
        public void StopOverriding_ShouldRemoveOverride()
        {
            //Arrange
            const int address = 0;
            const string expression = "x";
            const DataType dataType = DataType.UInt16;

            _dataGetterMock
                .Setup(getter => getter.Invoke(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ushort[] { 0 });

            //Act
            _dataOverrider.Override(address, expression, dataType);
            var ovrd = _dataOverrider.Overrides.First();
            _dataOverrider.StopOverriding(ovrd.Guid);

            //Assert
            _dataOverrider.Overrides
                .Should().BeEmpty();
        }

        [Fact]
        public void Write_ThereIsAnOverrideWithCorrectAddress_ShouldCallDataSetterWithChangedData()
        {
            //Arrange
            const int address = 0;
            const string expression = "x+1";
            const DataType dataType = DataType.UInt16;
            ushort[] data = new ushort[] { 1 };
            ushort[] changedData = new ushort[] { (ushort)(data[0] + 1) };

            _dataGetterMock
                .Setup(getter => getter.Invoke(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ushort[] { 0 });

            _dataOverrider.Override(address, expression, dataType);

            _dataSetterMock.Reset();
            
            //Act
            _dataOverrider.Write(address, data);

            //Assert
            _dataSetterMock.Verify(ds => ds.Invoke(address, changedData), Times.Once);
            _dataSetterMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Write_ThereIsAnOverrideWithWrongAddress_ShouldCallDataSetterWithOriginalData()
        {
            //Arrange
            const int address = 0;
            const int addressCalled = 10;
            const string expression = "x+1";
            const DataType dataType = DataType.UInt16;
            ushort[] data = new ushort[] { 1 };

            _dataGetterMock
                .Setup(getter => getter.Invoke(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ushort[] { 0 });

            _dataOverrider.Override(address, expression, dataType);

            _dataSetterMock.Reset();

            //Act
            _dataOverrider.Write(addressCalled, data);

            //Assert
            _dataSetterMock.Verify(ds => ds.Invoke(addressCalled, data), Times.Once);
            _dataSetterMock.VerifyNoOtherCalls();
        }

        // TODO: Add tests for overrides that are only
        // partly covering the data that is being written to
    }
}
