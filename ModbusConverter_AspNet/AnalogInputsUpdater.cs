using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.PeripheralDevices;
using EasyModbus;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace ModbusConverter
{
    public class AnalogInputsUpdater : BackgroundService
    {

        private readonly ModbusServer _modbusServer;
        private readonly RegistersToPeripheralsMap _registersToPeripheralsMap;
        private readonly IRegularPeripheralsManager _peripheralsManager;

        public AnalogInputsUpdater(
            ModbusServer modbusServer,
            RegistersToPeripheralsMap registersToPeripheralsMap,
            IRegularPeripheralsManager peripheralsManager)
        {
            _modbusServer = modbusServer;
            _registersToPeripheralsMap = registersToPeripheralsMap;
            _peripheralsManager = peripheralsManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UpdateAnalogInputs();
                await Task.Delay(1);
            }
        }

        void UpdateAnalogInputs()
        {
            foreach (AnalogInputChannel channel in Enum.GetValues(typeof(AnalogInputChannel)))
            {
                var inputValue = _peripheralsManager.ReadAnalogInput(channel);
                var registerAddress = _registersToPeripheralsMap.AnalogInputToInputRegisterMap[channel];

                var registers = BitConverter.GetBytes(inputValue);
                for (int i = 0; i < registers.Count(); ++i)
                {
                    _modbusServer.inputRegisters[registerAddress + i] = registers[i];
                }
            }
        }

    }
}
