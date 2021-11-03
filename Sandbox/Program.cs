using System;
using EasyModbus;
using System.Linq;
using org.mariuszgromada.math.mxparser;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var e = new Expression("~x");
            e.addArguments(new Argument("x"));
            e.setArgumentValue("x", -1.0);
            Console.WriteLine(e.calculate());
            
            e.setArgumentValue("x", 0.0);
            Console.WriteLine(e.calculate());

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
