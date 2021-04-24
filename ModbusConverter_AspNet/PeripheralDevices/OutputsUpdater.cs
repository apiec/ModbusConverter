using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModbusConverter.PeripheralDevices
{
    public class OutputsUpdater : IOutputsUpdater
    {
        private readonly IPeripheralsManager _peripheralsManager;

        public OutputsUpdater(IPeripheralsManager peripheralsManager)
        {
            _peripheralsManager = peripheralsManager;
        }

        public void OnCoilsChanged(int coil, int numberOfCoils)
        {
            var tasks = _peripheralsManager.OutputPeripherals
                .Where(peripheral => peripheral.RegisterType == ModbusRegisterType.Coil)
                .Where(peripheral => RangesOverlap(coil, numberOfCoils,
                                                   peripheral.RegisterAddress, peripheral.DataLengthInBools))
                .Select(peripheral => Task.Factory.StartNew(peripheral.ReadRegisterAndWriteToOutput));

            Task.WaitAll(tasks.ToArray());
        }

        public void OnHoldingRegistersChanged(int register, int numberOfRegisters)
        {
            var tasks = _peripheralsManager.OutputPeripherals
                .Where(peripheral => peripheral.RegisterType == ModbusRegisterType.HoldingRegister)
                .Where(peripheral => RangesOverlap(register, numberOfRegisters,
                                                   peripheral.RegisterAddress, peripheral.DataLengthInRegisters))
                .Select(peripheral => Task.Factory.StartNew(peripheral.ReadRegisterAndWriteToOutput));

            Task.WaitAll(tasks.ToArray());
        }

        private bool RangesOverlap(int startA, int lenA, int startB, int lenB)
        {
            return startA <= startB + lenB && startB <= startA + lenA;
        }
    }
}
