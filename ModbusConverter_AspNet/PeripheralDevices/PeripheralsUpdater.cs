using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusConverter.PeripheralDevices
{
    public class PeripheralsUpdater : BackgroundService
    {
        private readonly IPeripheralsManager _peripheralsManager;

        public PeripheralsUpdater(IPeripheralsManager peripheralsManager)
        {
            _peripheralsManager = peripheralsManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdatePeripherals();
            }
        }

        private async Task UpdatePeripherals()
        {
            var tasks = _peripheralsManager.Peripherals
                .Select(peripheral => Task.Factory.StartNew(() => peripheral.Update()));

            await Task.WhenAll(tasks);
        }

    }
}
