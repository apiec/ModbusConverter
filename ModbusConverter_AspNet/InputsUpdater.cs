using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.PeripheralDevices;
using EasyModbus;
using System.Threading;
using Microsoft.Extensions.Hosting;
using ModbusConverter.PeripheralDevices.Peripherals;

namespace ModbusConverter
{
    public class InputsUpdater : BackgroundService
    {
        private readonly PeripheralsManager _peripheralsManager;

        public InputsUpdater(PeripheralsManager peripheralsManager)
        {
            _peripheralsManager = peripheralsManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateInputPeripherals();
                await Task.Delay(1);
            }
        }

        private async Task UpdateInputPeripherals()
        {
            var tasks = _peripheralsManager.InputPeripherals
                .Where(peripheral => peripheral is not (AnalogInputChannel or AnalogOutputChannel))
                .Select(peripheral => Task.Factory.StartNew(() => peripheral.ReadAndSaveDataToRegister()));

            var sequentials = _peripheralsManager.InputPeripherals
                .Where(peripheral => peripheral is AnalogInputChannel or AnalogOutputChannel);

            foreach (var peripheral in sequentials)
            {
                peripheral.ReadAndSaveDataToRegister();
            }

            await Task.WhenAll(tasks);
        }

    }
}
