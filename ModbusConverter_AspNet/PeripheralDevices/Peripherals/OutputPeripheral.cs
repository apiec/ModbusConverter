using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyModbus;
using ModbusConverter.PeripheralDevices.Config;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public interface IOutputPeripheral : IPeripheral
    {
        void ReadRegisterAndWriteToOutput();
    }

    public abstract class OutputPeripheral<T> : IOutputPeripheral
    {
        private readonly IModbusServerWrapper _modbusServerWrapper;
        private readonly Dictionary<ModbusRegisterType, Func<T>> _readValueFuncs;

        public OutputPeripheral(IModbusServerWrapper modbusServerWrapper)
        {
            _modbusServerWrapper = modbusServerWrapper;
            _modbusServerWrapper.CoilsChanged += OnCoilsChanged;
            _modbusServerWrapper.HoldingRegistersChanged += OnHoldingRegistersChanged;

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

        #region EventHandling
        private void OnCoilsChanged(int coil, int numberOfCoils)
        {
            if (RegisterType == ModbusRegisterType.Coil &&
                RangesOverlap(coil, numberOfCoils, RegisterAddress, DataLengthInRegisters))
            {
                ReadRegisterAndWriteToOutput();
            }
        }

        private void OnHoldingRegistersChanged(int register, int numberOfRegisters)
        {
            if (RegisterType == ModbusRegisterType.HoldingRegister &&
                RangesOverlap(register, numberOfRegisters, RegisterAddress, DataLengthInRegisters))
            {
                ReadRegisterAndWriteToOutput();
            }
        }

        private bool RangesOverlap(int startA, int lenA, int startB, int lenB)
        {
            return startA <= startB + lenB && startB <= startA + lenA;
        }
        #endregion

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

        protected abstract int DataLengthInBools { get; }
        protected abstract int DataLengthInRegisters { get; }

        protected abstract T ReadValueFromBools(bool[] bools);
        protected abstract T ReadValueFromRegisters(ushort[] registers);

        protected abstract void WriteValueToOutput(T value);
    }
}
