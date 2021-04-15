using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Gpio;

namespace ModbusConverter.PeripheralDevices
{
    public interface IRegularPeripheralsManager
    {
        void SetOutputPin(DigitalOutputPin pin, PinValue pinValue);
        PinValue ReadInputPin(DigitalInputPin pin);

        void WriteToAnalogOutput(AnalogOutputChannel outputChannel, float value);
        float ReadAnalogInput(AnalogInputChannel inputChannel);

        void SetPwmChannel(int channel, double dutyCycle);
    }
}
