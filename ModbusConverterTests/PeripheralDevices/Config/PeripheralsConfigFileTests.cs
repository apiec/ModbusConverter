using Xunit;
using FluentAssertions;
using Moq;
using ModbusConverter.PeripheralDevices.Peripherals;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using ModbusConverter.PeripheralDevices.Config;
using System.IO;

namespace ModbusConverterTests.PeripheralDevices.Config
{
    public class PeripheralsConfigFileTests
    {
        const string _testDirectory = "TestFiles";

        MockFactory<IPeripheral> _iPeripheralMockFactory = new();
        Mock<IPeripheralsFactory> _peripheralsFactoryMock = new();

        PeripheralsConfigFile _peripheralsConfigFile;

        public PeripheralsConfigFileTests()
        {
            _peripheralsFactoryMock
                .Setup(f => f.CreateFromConfig(It.IsAny<PeripheralConfig>()))
                .Returns(() => _iPeripheralMockFactory.CreateMock().Object);

        }

        [Fact]
        public void ReadConfigFile_FileIsEmpty_ReturnsEmptyIEnumerableAndDoesntCallFactory()
        {
            //Arrange
            SetupTestCase("Empty.json");

            //Act
            var peripherals = _peripheralsConfigFile.ReadConfigFile();

            //Assert
            peripherals
                .Should()
                .BeEmpty()
                .And
                .BeEquivalentTo(_iPeripheralMockFactory.MockObjects);

            _peripheralsFactoryMock
                .VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("InputPin.json", typeof(InputPinConfig))]
        [InlineData("OutputPin.json", typeof(OutputPinConfig))]
        [InlineData("PwmPin.json", typeof(PwmPinConfig))]
        [InlineData("AnalogInputChannel.json", typeof(AnalogInputChannelConfig))]
        [InlineData("AnalogOutputChannel.json", typeof(AnalogOutputChannelConfig))]
        public void ReadConfigFile_FileContainsSinglePeripherla_DeserializesPeripheralConfigAndCallsPeripheralsFactory
            (string fileName, Type type)
        {
            //Arrange
            SetupTestCase(fileName);

            //Act
            var peripherals = _peripheralsConfigFile.ReadConfigFile();

            //Assert
            peripherals
                .Should()
                .HaveCount(1)
                .And
                .BeEquivalentTo(_iPeripheralMockFactory.MockObjects);

            _peripheralsFactoryMock
                .Verify(f => 
                    f.CreateFromConfig(It.Is<PeripheralConfig>(p => p.GetType() == type)),
                    Times.Once);
            
            _peripheralsFactoryMock
                .VerifyNoOtherCalls();
        }

        [Fact]
        public void ReadConfigFile_FileContainsEveryPeripheralOnce_DeserializesCorrectly()
        {
            //Arrange
            SetupTestCase("AllPeripherals.json");
            var types = new List<Type>
            {
                typeof(InputPinConfig),
                typeof(OutputPinConfig),
                typeof(AnalogInputChannelConfig),
                typeof(AnalogOutputChannelConfig),
                typeof(PwmPinConfig),
            };

            //Act
            var peripherals = _peripheralsConfigFile.ReadConfigFile();

            //Assert
            peripherals
                .Should()
                .HaveCount(types.Count)
                .And
                .BeEquivalentTo(_iPeripheralMockFactory.MockObjects);

            foreach (var type in types)
            {
                _peripheralsFactoryMock
                    .Verify(f =>
                        f.CreateFromConfig(It.Is<PeripheralConfig>(p => p.GetType() == type)),
                        Times.Once);
            }

            _peripheralsFactoryMock
                .VerifyNoOtherCalls();
        }

        [Fact]
        public void ReadConfigFile_PeripheralsAreInvalid_InvalidPeripheralsAreDiscarded()
        {
            //Arrange
            SetupTestCase("Invalid.json");
            
            //Act
            var peripherals = _peripheralsConfigFile.ReadConfigFile();

            //Assert
            peripherals
                .Should()
                .BeEquivalentTo(_iPeripheralMockFactory.MockObjects);

            _peripheralsFactoryMock
                .VerifyNoOtherCalls();
        }

        private void SetupTestCase(string fileName)
        {
            var path = Path.Combine(_testDirectory, fileName);

            var inMemorySettings = new Dictionary<string, string>
            {
                { "PeripheralsConfigFileName", path }
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _peripheralsConfigFile = new PeripheralsConfigFile(
                config, _peripheralsFactoryMock.Object);
        }
    }

    internal class MockFactory<T> where T : class
    {
        public IEnumerable<Mock<T>> Mocks { get => _mocks; }
        public IEnumerable<T> MockObjects { get => _mocks.Select(m => m.Object); }

        public Mock<T> CreateMock()
        {
            _mocks.Add(new Mock<T>());
            return _mocks.Last();
        }

        private List<Mock<T>> _mocks = new();
    }
}
