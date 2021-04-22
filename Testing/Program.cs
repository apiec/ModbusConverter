using System;
using System.Device.I2c;
using System.Threading;
using ModbusConverter.PeripheralDevices.AnalogIO;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("i2c");
            var i2cbus = I2cBus.Create(1);
            var i2cdevice = i2cbus.CreateDevice(0b1001000);
            var pcf = new PCF8591Device(i2cdevice);

            while (true)
            {
                var analog0 = pcf.ReadInput(PCF8591Device.InputMode.SingleEnded_AIN0);
                var analog1 = pcf.ReadInput(PCF8591Device.InputMode.SingleEnded_AIN1);
                var analog2 = pcf.ReadInput(PCF8591Device.InputMode.SingleEnded_AIN2);
                var analog3 = pcf.ReadInput(PCF8591Device.InputMode.SingleEnded_AIN3);

                var analog0_1 = pcf.ReadInput(PCF8591Device.InputMode.Differential_AIN0_AIN1);
                var analog2_3 = pcf.ReadInput(PCF8591Device.InputMode.Differential_AIN2_AIN3);


                pcf.WriteToOutput(analog0);
                Console.WriteLine($"{analog0}-{analog1}-{analog2}-{analog3}");
                Console.WriteLine($"{analog0_1}-{analog2_3}");
                Console.WriteLine("----------------------------------");

                Thread.Sleep(100);
            }
        }
    }
}
