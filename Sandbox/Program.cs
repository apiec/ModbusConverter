using System;
using EasyModbus;
using System.Linq;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
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
