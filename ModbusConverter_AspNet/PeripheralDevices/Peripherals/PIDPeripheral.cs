using EasyModbus;
using ModbusConverter.Modbus;
using ModbusConverter.PeripheralDevices.Config;
using System;
using System.Diagnostics;

namespace ModbusConverter.PeripheralDevices.Peripherals
{
    internal class PID
    {
        private double _previousError;
        private double _integral;

        public double Kp { get; set; }
        public double Ki { get; set; }
        public double Kd { get; set; }

        public double Max { get; set; }
        public double Min { get; set; }

        public double Calculate(double setpoint, double pv, double dt)
        {
            double error = setpoint - pv;

            double POut = Kp * error;

            _integral += error * dt;
            double IOut = Ki * _integral;

            double derivative = (error - _previousError) / dt;
            double DOut = Kd * derivative;
            double output = POut + IOut + DOut;

            if (output > Max)
            {
                output = Max;
            }
            else if (output < Min)
            {
                output = Min;
            }

            _previousError = error;

            return output;
        }
    }


    public class PIDPeripheral : IPeripheral
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly PID _pid = new();
        private readonly IModbusServerWrapper _modbusServerWrapper;

        public PIDPeripheral(IModbusServerWrapper modbusServerWrapper)
        {
            _modbusServerWrapper = modbusServerWrapper;
        }

        public ModbusRegisterType RegisterType { get => ModbusRegisterType.HoldingRegister; set => throw new NotSupportedException(); }
        public int RegisterAddress { get; set; }
        public string Name { get; set; }

        public void Update() => UpdatePID();

        public PeripheralConfig GetConfig()
        {
            throw new NotImplementedException();
        }

        private void UpdatePID()
        {
            _pid.Kp = Kp;
            _pid.Ki = Ki;
            _pid.Kd = Kd;
            _pid.Max = Max;
            _pid.Min = Min;

            var dt = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();
            CV = _pid.Calculate(SP, PV, dt);
        }

        public double Kp
        {
            get => ReadDoubleFromAddress(KpAddress);
            set => WriteDoubleToAddress(KpAddress, value);
        }
        public double Ki
        {
            get => ReadDoubleFromAddress(KiAddress);
            set => WriteDoubleToAddress(KiAddress, value);
        }
        public double Kd
        {
            get => ReadDoubleFromAddress(KdAddress);
            set => WriteDoubleToAddress(KdAddress, value);
        }
        public double Max
        {
            get => ReadDoubleFromAddress(MaxAddress);
            set => WriteDoubleToAddress(MaxAddress, value);
        }
        public double Min
        {
            get => ReadDoubleFromAddress(MinAddress);
            set => WriteDoubleToAddress(MinAddress, value);
        }
        public double SP
        {
            get => ReadDoubleFromAddress(SPAddress);
            set => WriteDoubleToAddress(SPAddress, value);
        }
        public double PV
        {
            get => ReadDoubleFromAddress(PVAddress);
            set => WriteDoubleToAddress(PVAddress, value);
        }
        public double CV
        {
            get => ReadDoubleFromAddress(CVAddress);
            set => WriteDoubleToAddress(CVAddress, value);
        }

        public int KpAddress => RegisterAddress;
        public int KiAddress => RegisterAddress + 1 * sizeof(double);
        public int KdAddress => RegisterAddress + 2 * sizeof(double);
        public int MaxAddress => RegisterAddress + 3 * sizeof(double);
        public int MinAddress => RegisterAddress + 4 * sizeof(double);
        public int SPAddress => RegisterAddress + 5 * sizeof(double);
        public int PVAddress => RegisterAddress + 6 * sizeof(double);
        public int CVAddress => RegisterAddress + 7 * sizeof(double);

        private double ReadDoubleFromAddress(int address)
        {
            var registers = _modbusServerWrapper.ReadHoldingRegisters(address, sizeof(double));
            var value = ModbusClient.ConvertRegistersToDouble(registers);
            return value;
        }

        private void WriteDoubleToAddress(int address, double value)
        {
            var registers = ModbusClient.ConvertDoubleToRegisters(value);
            _modbusServerWrapper.WriteToHoldingRegisters(address, registers);
        }

    }
}
