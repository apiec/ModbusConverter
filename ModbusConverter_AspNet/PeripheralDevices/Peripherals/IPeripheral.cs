using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyModbus;
using ModbusConverter.PeripheralDevices.Config;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public interface IPeripheral
    {
        ModbusRegisterType RegisterType { get; set; }
        int RegisterAddress { get; set; }
        string Name { get; set; }

        PeripheralConfig GetConfig();
    }

}
