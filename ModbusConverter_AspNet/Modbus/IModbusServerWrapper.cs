using EasyModbus;
using System.Collections.Generic;

namespace ModbusConverter.Modbus
{
    public interface IModbusServerWrapper
    {
        event ModbusServer.CoilsChangedHandler CoilsChanged;
        event ModbusServer.HoldingRegistersChangedHandler HoldingRegistersChanged;


        bool[] ReadCoils(int address, int numberOfRegisters);
        bool[] ReadDiscreteInputs(int address, int numberOfRegisters);
        ushort[] ReadHoldingRegisters(int address, int numberOfRegisters);
        ushort[] ReadInputRegisters(int address, int numberOfRegisters);

        void WriteToCoils(int address, bool[] registers);
        void WriteToDiscreteInputs(int address, bool[] registers);
        void WriteToHoldingRegisters(int address, ushort[] registers);
        void WriteToInputRegisters(int address, ushort[] registers);
    }
}