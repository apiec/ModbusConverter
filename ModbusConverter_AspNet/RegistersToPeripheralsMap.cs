using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.PeripheralDevices;
using static ModbusConverter.PeripheralDevices.Peripherals;

namespace ModbusConverter
{
    public class RegistersToPeripheralsMap
    {
        public Dictionary<int, DigitalInputPin> DiscreteInputToOutputPinMap { get; }
        public Dictionary<DigitalInputPin, int> InputPinToDiscreteInputMap { get; }

        public Dictionary<int, DigitalOutputPin> CoilToOutputPinMap { get; }
        public Dictionary<DigitalOutputPin, int> OutputPinToCoilMap { get; }

        public Dictionary<int, AnalogInputChannel> InputRegisterToAnalogInputMap { get; }
            = new Dictionary<int, AnalogInputChannel>();

        public Dictionary<AnalogInputChannel, int> AnalogInputToInputRegisterMap { get; }

        public Dictionary<int, AnalogOutputChannel> HoldingRegisterToAnalogOutputMap { get; }
            = new Dictionary<int, AnalogOutputChannel>();

        public RegistersToPeripheralsMap()
        {
            CoilToOutputPinMap = new Dictionary<int, DigitalOutputPin>()
            {
                {0, DigitalOutputPin.PIN_17},
                {1, DigitalOutputPin.PIN_27},
                {2, DigitalOutputPin.PIN_22},
                {3, DigitalOutputPin.PIN_23},
                {4, DigitalOutputPin.PIN_25},
                {5, DigitalOutputPin.PIN_5},
                {6, DigitalOutputPin.PIN_6},
                {7, DigitalOutputPin.PIN_16}
            };

            OutputPinToCoilMap = CoilToOutputPinMap
                .ToDictionary(coilPinPair => coilPinPair.Value, coilPinPair => coilPinPair.Key);




        }

    }
}
