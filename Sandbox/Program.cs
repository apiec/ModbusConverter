using System;
using EasyModbus;
using System.Linq;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(BitConverter.IsLittleEndian);

            var oneInRegisters = ModbusClient.ConvertFloatToRegisters(1f);
            var halfInRegisters = ModbusClient.ConvertFloatToRegisters(0.5f);

            PrintRegisters(oneInRegisters);
            PrintRegisters(halfInRegisters);

            var oneInBytes = BitConverter.GetBytes(1f);
            var halfInBytes = BitConverter.GetBytes(0.5f);
            PrintBytes(oneInBytes);
            PrintBytes(halfInBytes);

            PrintRegisters(new ushort[] { BitConverter.ToUInt16(oneInBytes, 0), BitConverter.ToUInt16(oneInBytes, 2) });
            PrintRegisters(new ushort[] { BitConverter.ToUInt16(halfInBytes, 0), BitConverter.ToUInt16(halfInBytes, 2) });

            PrintBytes(BitConverter.GetBytes((ushort)0x803f));
        }

        static void PrintRegisters(ushort[] registers)
        {
            var strs = registers.Select(register => register.ToString("X4"));

            Console.WriteLine(string.Join('-', strs));
        }
        static void PrintBytes(byte[] bytes)
        {
            var strs = bytes.Select(@byte => @byte.ToString("X2"));

            Console.WriteLine(string.Join('-', strs));
        }

    }
}
