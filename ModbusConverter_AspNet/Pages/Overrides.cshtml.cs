using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModbusConverter.Modbus;

namespace ModbusConverter.Pages
{


    public class OverridesRow
    {
        public int? CoilAddress { get; set; }
        public bool? CoilValue { get; set; }
        public int? DiscreteInputAddress { get; set; }
        public bool? DiscreteInputValue { get; set; }
        public int? InputRegisterAddress { get; set; }
        public int? InputRegisterValue { get; set; }
        public int? HoldingRegisterAddress { get; set; }
        public int? HoldingRegisterValue { get; set; }
    }



    public class OverridesModel : PageModel
    {

        public OverridesModel(IModbusOverrider modbusOverrider)
        {
            ModbusOverrider = modbusOverrider;
        }
        public IModbusOverrider ModbusOverrider { get; set; }
        [BindProperty] public int Address { get; set; }
        [BindProperty] public string Value { get; set; }
        [BindProperty] public bool BoolValue { get; set; }
        [BindProperty] public string RegisterType { get; set; }

        public List<OverridesRow> Rows
        {
            get
            {
                var result = new List<OverridesRow>();
                var coilOverrides = ModbusOverrider.GetCoilOverrides()
                    .OrderBy(p => p.Key)
                    .ToArray();
                var discreteOverrides = ModbusOverrider.GetDiscreteInputOverrides()
                    .OrderBy(p => p.Key)
                    .ToArray();
                var inputOverrides = ModbusOverrider.GetInputRegisterOverrides()
                    .OrderBy(p => p.Key)
                    .ToArray();
                var holdingOverrides = ModbusOverrider.GetHoldingRegisterOverrides()
                    .OrderBy(p => p.Key)
                    .ToArray();

                var lens = new int[] { coilOverrides.Count(), discreteOverrides.Count(), inputOverrides.Count(), holdingOverrides.Count() };

                for (int i = 0; i < lens.Max(); ++i)
                {
                    result.Add(new OverridesRow
                    {
                        CoilAddress = i < coilOverrides.Count() ? coilOverrides[i].Key : null,
                        CoilValue = i < coilOverrides.Count() ? coilOverrides[i].Value : null,
                        DiscreteInputAddress =  i < discreteOverrides.Count() ? discreteOverrides[i].Key : null,
                        DiscreteInputValue = i < discreteOverrides.Count() ? discreteOverrides[i].Value : null,
                        InputRegisterAddress = i < inputOverrides.Count() ? inputOverrides[i].Key : null,
                        InputRegisterValue = i < inputOverrides.Count() ? inputOverrides[i].Value : null,
                        HoldingRegisterAddress = i < holdingOverrides.Count() ? holdingOverrides[i].Key : null,
                        HoldingRegisterValue = i < holdingOverrides.Count() ? holdingOverrides[i].Value : null,
                    });
                }

                return result;
            }
        }



        public void OnPostAdd()
        {
            if (Address < 1 || Address > 9999)
                return;

            ModbusRegisterType registerType;
            try
            {
                registerType = (ModbusRegisterType)Enum.Parse(typeof(ModbusRegisterType), RegisterType);
            }
            catch (Exception)
            {
                return;
            }

            var boolDict = new Dictionary<int, bool>();
            var ushortDict = new Dictionary<int, ushort>();
            var value = ushort.Parse(Value, System.Globalization.NumberStyles.HexNumber);
            switch (registerType)
            {
                case ModbusRegisterType.Coil:
                    boolDict.Add(Address, BoolValue);
                    ModbusOverrider.OverrideCoils(boolDict);
                    break;
                case ModbusRegisterType.DiscreteInput:
                    boolDict.Add(Address, BoolValue);
                    ModbusOverrider.OverrideDiscreteInputs(boolDict);
                    break;
                case ModbusRegisterType.InputRegister:
                    ushortDict.Add(Address, value);
                    ModbusOverrider.OverrideInputRegisters(ushortDict);
                    break;
                case ModbusRegisterType.HoldingRegister:
                    ushortDict.Add(Address, value);
                    ModbusOverrider.OverrideHoldingRegisters(ushortDict);
                    break;
            }
            Address = 0;
            BoolValue = false;
            Value = string.Empty;
        }

        public void OnPostRemove()
        {
            if (Address < 1 || Address > 9999)
                return;

            ModbusRegisterType registerType;
            try
            {
                registerType = (ModbusRegisterType)Enum.Parse(typeof(ModbusRegisterType), RegisterType);
            }
            catch (Exception)
            {
                return;
            }

            var array = new int[] { Address };
            switch (registerType)
            {
                case ModbusRegisterType.Coil:
                    ModbusOverrider.StopOverridingCoils(array);
                    break;
                case ModbusRegisterType.DiscreteInput:
                    ModbusOverrider.StopOverridingDiscreteInputs(array);
                    break;
                case ModbusRegisterType.InputRegister:
                    ModbusOverrider.StopOverridingInputRegisters(array);
                    break;
                case ModbusRegisterType.HoldingRegister:
                    ModbusOverrider.StopOverridingHoldingRegisters(array);
                    break;
            }

            Address = 0;
            BoolValue = false;
            Value = string.Empty;
        }
    }
}
