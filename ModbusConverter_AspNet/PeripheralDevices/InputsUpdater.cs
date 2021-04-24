using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace ModbusConverter.PeripheralDevices
{
    public class InputsUpdater : BackgroundService
    {
        private readonly IPeripheralsManager _peripheralsManager;

        public InputsUpdater(IPeripheralsManager peripheralsManager)
        {
            _peripheralsManager = peripheralsManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateInputPeripherals();
            }
        }

        private async Task UpdateInputPeripherals()
        {
            var tasks = _peripheralsManager.InputPeripherals
                .Select(peripheral => Task.Factory.StartNew(() => peripheral.ReadAndSaveDataToRegister()));

            await Task.WhenAll(tasks);
        }

    }
}
