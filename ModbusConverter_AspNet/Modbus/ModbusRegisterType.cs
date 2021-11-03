using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModbusConverter.Modbus
{
    public enum ModbusRegisterType
    {
        Coil,
        DiscreteInput,
        InputRegister,
        HoldingRegister
    }
}
