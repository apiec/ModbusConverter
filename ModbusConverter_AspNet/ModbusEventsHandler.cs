using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyModbus;
using ModbusConverter.PeripheralDevices;
using System.Device.Gpio;

namespace ModbusConverter
{
    public class ModbusEventsHandler : IModbusEventsHandler
    {
        private readonly ModbusServer _modbusServer;
        private readonly RegistersToPeripheralsMap _registersToPeripheralsMap;
        private readonly IRegularPeripheralsManager _peripheralsManager;

        public ModbusEventsHandler(
            ModbusServer modbusServer,
            RegistersToPeripheralsMap registersToPeripheralsMap,
            IRegularPeripheralsManager peripheralsManager)
        {
            _modbusServer = modbusServer;
            _registersToPeripheralsMap = registersToPeripheralsMap;
            _peripheralsManager = peripheralsManager;
        }

        public void CoilsChangedHandler(int coil, int numberOfCoils)
        {
            for (int i = 0; i < numberOfCoils; ++i)
            {
                var currentCoil = coil + i;
                if (_registersToPeripheralsMap.CoilToOutputPinMap.TryGetValue(currentCoil, out var pin))
                {
                    var coilValue = _modbusServer.coils[currentCoil];

                    _peripheralsManager.SetOutputPin(pin, coilValue);
                }
            }
        }

        public void HoldingRegistersChangedHandler(int register, int numberOfRegisters)
        {
            //TODO
        }

    }
}
