using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModbusConverter.PeripheralDevices
{
    public class AnalogInterface : IAnalogInterface
    {
        public float ReadInput(AnalogInputChannel channel)
        {
            throw new NotImplementedException();
        }

        public void WriteToOutput(AnalogOutputChannel channel, float value)
        {
            throw new NotImplementedException();
        }
    }
}
