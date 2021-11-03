using EasyModbus;
using ModbusConverter.PeripheralDevices.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.Modbus;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public abstract class InputPeripheral : IPeripheral
    {
        private readonly IModbusServerWrapper _modbusServerWrapper;
        private readonly Dictionary<ModbusRegisterType, Action> _saveDataActions;

        public InputPeripheral(IModbusServerWrapper modbusServerWrapper)
        {
            _modbusServerWrapper = modbusServerWrapper;
            _saveDataActions = new Dictionary<ModbusRegisterType, Action>
            {
                { ModbusRegisterType.Coil, SaveDataToCoils },
                { ModbusRegisterType.DiscreteInput, SaveDataToDiscreteInputs },
                { ModbusRegisterType.InputRegister, SaveDataToInputRegisters },
                { ModbusRegisterType.HoldingRegister, SaveDataToHoldingRegisters }
            };
        }

        public ModbusRegisterType RegisterType { get; set; }
        public int RegisterAddress { get; set; }
        public string Name { get; set; }

        public abstract PeripheralConfig GetConfig();

        public void Update() => ReadAndSaveDataToRegister();
        public void ReadAndSaveDataToRegister()
        {
            var saveDataAction = _saveDataActions[RegisterType];
            saveDataAction();
        }

        private void SaveDataToCoils()
        {
            var data = ReadDataAsBools();
            _modbusServerWrapper.WriteToCoils(RegisterAddress, data);
        }

        private void SaveDataToDiscreteInputs()
        {
            var data = ReadDataAsBools();
            _modbusServerWrapper.WriteToDiscreteInputs(RegisterAddress, data);
        }

        private void SaveDataToInputRegisters()
        {
            var data = ReadDataAsUshorts();
            _modbusServerWrapper.WriteToInputRegisters(RegisterAddress, data);
        }

        private void SaveDataToHoldingRegisters()
        {
            var data = ReadDataAsUshorts();
            _modbusServerWrapper.WriteToHoldingRegisters(RegisterAddress, data);
        }

        protected abstract bool[] ReadDataAsBools();
        protected abstract ushort[] ReadDataAsUshorts();
    }
}
