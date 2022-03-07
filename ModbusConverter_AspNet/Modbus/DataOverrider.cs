using System;
using System.Collections.Generic;
using System.Linq;

namespace ModbusConverter.Modbus
{
    public class DataOverrider
    {
        private readonly Action<int, ushort[]> _dataSetter;
        private readonly Func<int, int, ushort[]> _dataGetter;
        private readonly Dictionary<Guid, DynamicOverride> _overrides = new();

        public DataOverrider(Action<int, ushort[]> dataSetter, Func<int, int, ushort[]> dataGetter)
        {
            _dataSetter = dataSetter;
            _dataGetter = dataGetter;
        }

        public void Write(int address, ushort[] data)
        {
            var overrides = _overrides.Values
                .Where(o => DoRangesOverlap(address, address + data.Length, o.Address, o.Address + o.LengthOfDataTypeInRegisters))
                .OrderBy(o => o.Address);

            var overriden = new bool[data.Length];

            foreach (var @override in overrides)
            {
                ushort[] newData = _dataGetter(@override.Address, @override.LengthOfDataTypeInRegisters);
                int sourceOffset;
                int length;
                int destinationOffset;

                if (@override.Address < address)
                {
                    sourceOffset = 0;
                    length = address - @override.Address;
                    destinationOffset = @override.LengthOfDataTypeInRegisters - length;
                }
                else
                if (@override.Address + @override.LengthOfDataTypeInRegisters > address + data.Length)
                {
                    sourceOffset = address + data.Length;
                    length = @override.Address + @override.LengthOfDataTypeInRegisters - sourceOffset;
                    destinationOffset = 0;
                }
                else
                {
                    sourceOffset = @override.Address - address;
                    length = @override.LengthOfDataTypeInRegisters;
                    destinationOffset = 0;
                }

                Array.Copy(data, sourceOffset, newData, destinationOffset, length);
                ushort[] result;
                try
                {
                    result = @override.Calculate(newData);
                }
                catch (Exception)
                {
                    result = newData;
                }

                _dataSetter(@override.Address, result);

                for (int i = sourceOffset; i < sourceOffset + length && i < data.Length; ++i)
                {
                    overriden[i] = true;
                }
            }

            int currentAddress = address;
            List<ushort> currentData = new();
            for (int i = 0; i < overriden.Length; ++i)
            {
                if (overriden[i] is false)
                {
                    if (currentData.Any() is false)
                    {
                        currentAddress = address + i;
                    }
                    currentData.Add(data[i]);
                }
                else
                {
                    if (currentData.Any())
                    {
                        _dataSetter(currentAddress, currentData.ToArray());
                        currentData.Clear();
                    }
                }
            }

            if (currentData.Any())
            {
                _dataSetter(currentAddress, currentData.ToArray());
            }
        }


        public void Override(int address, string expression, DataType dataType)
        {
            var @override = new DynamicOverride
            {
                Address = address,
                DataType = dataType,
                OverrideExpression = expression
            };

            _overrides.Add(@override.Guid, @override);

            var data = _dataGetter(@override.Address, @override.LengthOfDataTypeInRegisters);
            Write(address, data);
        }

        public void StopOverriding(Guid guid)
        {
            var @override = _overrides[guid];
            _overrides.Remove(guid);

            var data = _dataGetter(@override.Address, @override.LengthOfDataTypeInRegisters);
            Write(@override.Address, data);
        }

        public IEnumerable<DynamicOverride> Overrides => _overrides.Values.AsEnumerable();

        private static bool DoRangesOverlap(int startA, int endA, int startB, int endB)
            => startA <= endB && endA >= startB;

    }
}
