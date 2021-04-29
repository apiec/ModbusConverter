using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyModbus;
using ModbusConverter.PeripheralDevices.Config;
using ModbusConverter.Modbus;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public interface IOutputPeripheral : IPeripheral
    {
        int DataLengthInBools { get; }
        int DataLengthInRegisters { get; }

        void ReadRegisterAndWriteToOutput();
    }

    public abstract class OutputPeripheral<T> : IOutputPeripheral
        where T : unmanaged
    {
        private readonly IModbusServerWrapper _modbusServerWrapper;
        private readonly Dictionary<ModbusRegisterType, Func<T>> _readValueFuncs;

        public OutputPeripheral(IModbusServerWrapper modbusServerWrapper)
        {
            _modbusServerWrapper = modbusServerWrapper;

            _readValueFuncs = new Dictionary<ModbusRegisterType, Func<T>>
            {
                { ModbusRegisterType.Coil, ReadValueFromCoils},
                { ModbusRegisterType.DiscreteInput, ReadValueFromDiscreteInputs},
                { ModbusRegisterType.InputRegister, ReadValueFromInputRegisters },
                { ModbusRegisterType.HoldingRegister, ReadValueFromHoldingRegisters }
            };
        }

        public ModbusRegisterType RegisterType { get; set; }
        public int RegisterAddress { get; set; }
        public string Name { get; set; }

        public abstract PeripheralConfig GetConfig();


        public void ReadRegisterAndWriteToOutput()
        {
            var readValueFunc = _readValueFuncs[RegisterType];
            var value = readValueFunc();

            WriteValueToOutput(value);
        }

        private T ReadValueFromCoils()
        {
            var bools = _modbusServerWrapper.ReadCoils(RegisterAddress, DataLengthInBools);
            var value = ReadValueFromBools(bools);

            return value;
        }

        private T ReadValueFromDiscreteInputs()
        {
            var bools = _modbusServerWrapper.ReadDiscreteInputs(RegisterAddress, DataLengthInBools);
            var value = ReadValueFromBools(bools);

            return value;
        }

        private T ReadValueFromInputRegisters()
        {
            var registers = _modbusServerWrapper.ReadInputRegisters(RegisterAddress, DataLengthInRegisters);
            var value = ReadValueFromRegisters(registers);

            return value;
        }

        private T ReadValueFromHoldingRegisters()
        {
            var registers = _modbusServerWrapper.ReadHoldingRegisters(RegisterAddress, DataLengthInRegisters);
            var value = ReadValueFromRegisters(registers);

            return value;
        }

        public virtual unsafe int DataLengthInBools
        {
            get
            {
                if (typeof(T) == typeof(bool))
                {
                    return 1;
                }
                else
                {
                    throw new NotSupportedException("Only a bool can have a length in bools at the moment");
                }
            }
        }
        
        public virtual unsafe int DataLengthInRegisters => (sizeof(T) + sizeof(ushort) - 1) / sizeof(ushort);

        protected abstract T ReadValueFromBools(bool[] bools);
        protected abstract T ReadValueFromRegisters(ushort[] registers);

        protected abstract void WriteValueToOutput(T value);
    }
}
