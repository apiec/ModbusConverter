﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.PeripheralDevices.AnalogIO;
using EasyModbus;
using ModbusConverter.PeripheralDevices.Config;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    public class AnalogInputChannel : InputPeripheral
    {
        private readonly IAnalogIOController _analogIOController;

        public AnalogInputChannel(IAnalogIOController analogIOController, IModbusServerWrapper modbusServerProxy)
            : base(modbusServerProxy)
        {
            _analogIOController = analogIOController;
        }

        public int PCF8591Number { get; set; }
        public PCF8591Device.InputMode InputMode { get; set; }

        public override PeripheralConfig GetConfig()
        {
            return new AnalogInputChannelConfig(this);
        }

        protected override bool[] ReadDataAsBools()
        {
            throw new NotSupportedException("AnalogInputChannel does not support reading data as bools");
        }

        protected override ushort[] ReadDataAsUshorts()
        {
            var data = ReadData();
            return new ushort[] { data };
        }

        private byte ReadData()
        {
            return _analogIOController.ReadInput(PCF8591Number, InputMode);
        }
    }
}
