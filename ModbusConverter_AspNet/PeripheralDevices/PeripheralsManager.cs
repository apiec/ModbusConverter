using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Device.Gpio;
using System.Collections.Concurrent;

namespace ModbusConverter.PeripheralDevices
{
    public class PeripheralsManager : IRegularPeripheralsManager, IOverridingPeripheralsManager
    {
        private readonly ConcurrentDictionary<DigitalInputPin, PinValue> _inputPinOverridesMap;
        private readonly ConcurrentDictionary<DigitalOutputPin, PinValue> _outputPinOverridesMap;
        private readonly ConcurrentDictionary<int, double> _pwmOverridesMap;
        private readonly ConcurrentDictionary<AnalogInputChannel, float> _inputChannelOverridesMap;
        private readonly ConcurrentDictionary<AnalogOutputChannel, float> _outputChannelOverridesMap;

        private readonly Peripherals _peripherals;

        public PeripheralsManager(Peripherals peripherals)
        {
            _inputChannelOverridesMap = new ConcurrentDictionary<AnalogInputChannel, float>();
            _outputChannelOverridesMap = new ConcurrentDictionary<AnalogOutputChannel, float>();
            _pwmOverridesMap = new ConcurrentDictionary<int, double>();
            _inputChannelOverridesMap = new ConcurrentDictionary<AnalogInputChannel, float>();
            _outputChannelOverridesMap = new ConcurrentDictionary<AnalogOutputChannel, float>();

            _peripherals = peripherals;
        }

        public void SetOutputPin(DigitalOutputPin pin, PinValue pinValue)
        {
            if (!_outputPinOverridesMap.ContainsKey(pin))
            {
                _peripherals.SetOutputPin(pin, pinValue);
            }
            else
            {
                _peripherals.SetOutputPin(pin, _outputPinOverridesMap[pin]);
            }
        }

        public PinValue ReadInputPin(DigitalInputPin pin)
        {
            if (!_inputPinOverridesMap.ContainsKey(pin))
            {
                return _peripherals.ReadInputPin(pin);
            }
            else
            {
                return _inputPinOverridesMap[pin];
            }
        }

        public void WriteToAnalogOutput(AnalogOutputChannel outputChannel, float value)
        {

            if (!_outputChannelOverridesMap.ContainsKey(outputChannel))
            {
                _peripherals.WriteToAnalogOutput(outputChannel, value);
            }
            else
            {
                _peripherals.WriteToAnalogOutput(outputChannel, _outputChannelOverridesMap[outputChannel]);
            }
        }

        public float ReadAnalogInput(AnalogInputChannel inputChannel)
        {
            if (!_inputChannelOverridesMap.ContainsKey(inputChannel))
            {
                return _peripherals.ReadAnalogInput(inputChannel);
            }
            else
            {
                return _inputChannelOverridesMap[inputChannel];
            }
        }

        public void SetPwmChannel(int channel, double dutyCycle)
        {
            if (channel < 0 || channel >= _peripherals.PwmChannels.Count())
            {
                throw new ArgumentOutOfRangeException();
            }

            if (!_pwmOverridesMap.ContainsKey(channel))
            {
                _peripherals.PwmChannels[channel].DutyCycle = dutyCycle;
            }
            else
            {
                _peripherals.PwmChannels[channel].DutyCycle = _pwmOverridesMap[channel];
            }
        }

        public void OverrideInputPin(DigitalInputPin pin, PinValue pinValue)
        {
            _inputPinOverridesMap[pin] = pinValue;
        }

        public void StopOverridingInputPin(DigitalInputPin pin)
        {
            _inputPinOverridesMap.TryRemove(pin, out _);
        }

        public void OverrideOutputPin(DigitalOutputPin pin, PinValue pinValue)
        {
            _outputPinOverridesMap[pin] = pinValue;
        }

        public void StopOverridingOutputPin(DigitalOutputPin pin)
        {
            _outputPinOverridesMap.TryRemove(pin, out _);
        }

        public void OverridePwmChannel(int channel, double dutyCycle)
        {
            _pwmOverridesMap[channel] = dutyCycle;
        }

        public void StopOverridingPwmChannel(int channel)
        {
            _pwmOverridesMap.TryRemove(channel, out _);
        }

        public void OverrideInputChannel(AnalogInputChannel inputChannel, float value)
        {
            _inputChannelOverridesMap[inputChannel] = value;
        }

        public void StopOverridingInputChannel(AnalogInputChannel inputChannel)
        {
            _inputChannelOverridesMap.TryRemove(inputChannel, out _);
        }

        public void OverrideOutputChannel(AnalogOutputChannel outputChannel, float value)
        {
            _outputChannelOverridesMap[outputChannel] = value;
        }

        public void StopOverridingOutputChannel(AnalogOutputChannel outputChannel)
        {
            _outputChannelOverridesMap.TryRemove(outputChannel, out _);
        }
    }
}
