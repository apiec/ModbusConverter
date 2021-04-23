using EasyModbus;
using ModbusConverter.PeripheralDevices.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public interface IInputPeripheral : IPeripheral
    {
        void ReadAndSaveDataToRegister();
    }

    public abstract class InputPeripheral : IInputPeripheral
    {
        private readonly ModbusServerWrapper _modbusServerProxy;
        private readonly Dictionary<ModbusRegisterType, Action> _saveDataActions;

        public InputPeripheral(IModbusServerWrapper modbusServerProxy)
        {
            _modbusServerProxy = modbusServerProxy;
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

        public void ReadAndSaveDataToRegister()
        {
            var saveDataAction = _saveDataActions[RegisterType];
            saveDataAction();
        }

        private void SaveDataToCoils()
        {
            var data = ReadDataAsBools();
            _modbusServerProxy.WriteToCoils(RegisterAddress, data);
        }

        private void SaveDataToDiscreteInputs()
        {
            var data = ReadDataAsBools();
            _modbusServerProxy.WriteToDiscreteInputs(RegisterAddress, data);
        }

        private void SaveDataToInputRegisters()
        {
            var data = ReadDataAsUshorts();
            _modbusServerProxy.WriteToInputRegisters(RegisterAddress, data);
        }

        private void SaveDataToHoldingRegisters()
        {
            var data = ReadDataAsUshorts();
            _modbusServerProxy.WriteToHoldingRegisters(RegisterAddress, data);
        }

        protected abstract bool[] ReadDataAsBools();
        protected abstract ushort[] ReadDataAsUshorts();
    }
}
