using Microsoft.Extensions.Configuration;
using System;
using System.Device.I2c;

namespace ModbusConverter.PeripheralDevices.AnalogIO
{
    public class PCF8591DeviceFactory : IPCF8591DeviceFactory
    {
        private readonly I2cBus _i2cBus;

        public PCF8591DeviceFactory(IConfiguration configuration)
        {
            _i2cBus = I2cBus.Create(configuration.GetValue<int>("I2CBusId"));
        }

        public PCF8591Device CreatePCF8591Device(int address)
        {
            if (address < 0 || address > 7)
            {
                throw new ArgumentOutOfRangeException("PCF8591 addresses have to be in range 0-7");
            }

            var i2cAddress = 0b1001000 | address;
            var i2cDevice = _i2cBus.CreateDevice(i2cAddress);

            var pcfDevice = new PCF8591Device(i2cDevice);

            return pcfDevice;
        }
    }
}
