using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Gpio;

namespace ModbusConverter.PeripheralDevices
{
    public interface IOverridingPeripheralsManager
    {
        void OverrideInputPin(DigitalInputPin pin, PinValue pinValue);
        void StopOverridingInputPin(DigitalInputPin pin);

        void OverrideOutputPin(DigitalOutputPin pin, PinValue pinValue);
        void StopOverridingOutputPin(DigitalOutputPin pin);

        void OverridePwmChannel(int channel, double dutyCycle);
        void StopOverridingPwmChannel(int channel);

        void OverrideInputChannel(AnalogInputChannel inputChannel, float value);
        void StopOverridingInputChannel(AnalogInputChannel inputChannel);

        void OverrideOutputChannel(AnalogOutputChannel outputChannel, float value);
        void StopOverridingOutputChannel(AnalogOutputChannel outputChannel);
    }
}
