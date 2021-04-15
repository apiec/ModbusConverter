using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyModbus;
using ModbusConverter.PeripheralDevices;
using System.Device.Gpio;
using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace ModbusConverter
{
    public class DigitalInputsUpdater : BackgroundService
    {
        private readonly ModbusServer _modbusServer;
        private readonly RegistersToPeripheralsMap _registersToPeripheralsMap;
        private readonly IRegularPeripheralsManager _peripheralsManager;

        private readonly DigitalInputPin[] _pins;

        public DigitalInputsUpdater(
            ModbusServer modbusServer,
            RegistersToPeripheralsMap registersToPeripheralsMap,
            IRegularPeripheralsManager peripheralsManager)
        {
            _modbusServer = modbusServer;
            _registersToPeripheralsMap = registersToPeripheralsMap;
            _peripheralsManager = peripheralsManager;

            _pins = (DigitalInputPin[])Enum.GetValues(typeof(DigitalInputPin));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateDigitalInputs();
                await Task.Delay(1);
            }
        }

        private async Task UpdateDigitalInputs()
        {
            var tasks = _pins.Select(pin =>
                Task.Factory.StartNew(() => UpdateDigitalInput(pin)));
            await Task.WhenAll(tasks);
        }

        void UpdateDigitalInput(DigitalInputPin pin)
        {
            var pinValue = _peripheralsManager.ReadInputPin(pin);
            var inputAddress = _registersToPeripheralsMap.InputPinToDiscreteInputMap[pin];

            _modbusServer.discreteInputs[inputAddress] = (bool)pinValue;
        }
    }
}
