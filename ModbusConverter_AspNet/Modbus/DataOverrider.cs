using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModbusConverter.Modbus
{
    public class DataOverrider<TData>
    {
        private readonly Action<int, TData> _dataSetter;
        private readonly Func<int, TData> _dataGetter;
        private readonly Dictionary<int, TData> _overrides = new();

        public DataOverrider(Action<int, TData> dataSetter, Func<int, TData> dataGetter)
        {
            _dataSetter = dataSetter;
            _dataGetter = dataGetter;
        }

        public void Write(int address, TData data)
        {
            var dataToSet = _overrides.ContainsKey(address)
                ? _overrides[address]
                : data;

            _dataSetter(address, dataToSet);
        }

        public TData Read(int address)
        {
            return _overrides.ContainsKey(address)
                ? _overrides[address]
                : _dataGetter(address);
        }

        public void Override(int address, TData data)
        {
            _overrides[address] = data;
        }

        public void StopOverriding(int address)
        {
            _overrides.Remove(address);
        }

        public IEnumerable<KeyValuePair<int, TData>> Overrides => _overrides.AsEnumerable();
    }
}
