using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModbusConverter.PeripheralDevices.AnalogIO
{
    public class AnalogIOController : IAnalogIOController
    {
        private readonly List<PCF8591Device> _pcf8591Devices;

        public AnalogIOController(PCF8591DeviceFactory pcf8591DeviceFactory, IConfiguration configuration)
        {
            _pcf8591Devices = new List<PCF8591Device>();

            PCF8591Count = configuration.GetValue<int>("PCF8591Count");

            for (int i = 0; i < PCF8591Count; ++i)
            {
                var newPcfDevice = pcf8591DeviceFactory.CreatePCF8591Device(i);
                _pcf8591Devices.Add(newPcfDevice);
            }

        }

        public int PCF8591Count { get; }

        public byte ReadInput(int pcf8591Number, PCF8591Device.InputMode inputMode)
        {
            CheckIfPCFNumberInBounds(pcf8591Number);

            var device = _pcf8591Devices[pcf8591Number];
            byte input = 0;
            lock(device)
            {
                input = device.ReadInput(inputMode);
            }
            
            return input;
        }

        public void WriteToOutput(int pcf8591Number, byte value)
        {
            CheckIfPCFNumberInBounds(pcf8591Number);

            var device = _pcf8591Devices[pcf8591Number];
            lock (device)
            {
                device.WriteToOutput(value);
            }
        }

        private void CheckIfPCFNumberInBounds(int pcfNumber)
        {
            if (pcfNumber < 0 || pcfNumber >= PCF8591Count)
            {
                throw new ArgumentOutOfRangeException($"The pcf8591Number: {pcfNumber} is out of bounds");
            }
        }

    }
}
