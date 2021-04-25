using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ModbusConverter.Pages
{
    public class OverridesModel : PageModel
    {

        public OverridesModel(IModbusServerWrapper modbusServerWrapper)
        {
            ModbusServerWrapper = modbusServerWrapper;
        }
        public IModbusServerWrapper ModbusServerWrapper { get; set; }
        [BindProperty] public int Address { get; set; }
        [BindProperty] public ushort Value { get; set; }
        [BindProperty] public bool BoolValue { get; set; }
        [BindProperty] public string RegisterType { get; set; }


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
        public List<OverridesRow> Rows
        {
            get
            {
                var result = new List<OverridesRow>();
                var coilOverrides = ModbusServerWrapper.GetCoilOverrides()
                    .OrderBy(p => p.Key)
                    .ToArray();
                var discreteOverrides = ModbusServerWrapper.GetDiscreteInputOverrides()
                    .OrderBy(p => p.Key)
                    .ToArray();
                var inputOverrides = ModbusServerWrapper.GetInputRegisterOverrides()
                    .OrderBy(p => p.Key)
                    .ToArray();
                var holdingOverrides = ModbusServerWrapper.GetHoldingRegisterOverrides()
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
            switch (registerType)
            {
                case ModbusRegisterType.Coil:
                    boolDict.Add(Address, BoolValue);
                    ModbusServerWrapper.OverrideCoils(boolDict);
                    break;
                case ModbusRegisterType.DiscreteInput:
                    boolDict.Add(Address, BoolValue);
                    ModbusServerWrapper.OverrideDiscreteInputs(boolDict);
                    break;
                case ModbusRegisterType.InputRegister:
                    ushortDict.Add(Address, Value);
                    ModbusServerWrapper.OverrideInputRegisters(ushortDict);
                    break;
                case ModbusRegisterType.HoldingRegister:
                    ushortDict.Add(Address, Value);
                    ModbusServerWrapper.OverrideHoldingRegisters(ushortDict);
                    break;
            }
            Address = 0;
            BoolValue = false;
            Value = 0;
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
                    ModbusServerWrapper.StopOverridingCoils(array);
                    break;
                case ModbusRegisterType.DiscreteInput:
                    ModbusServerWrapper.StopOverridingDiscreteInputs(array);
                    break;
                case ModbusRegisterType.InputRegister:
                    ModbusServerWrapper.StopOverridingInputRegisters(array);
                    break;
                case ModbusRegisterType.HoldingRegister:
                    ModbusServerWrapper.StopOverridingHoldingRegisters(array);
                    break;
            }

            Address = 0;
            BoolValue = false;
            Value = 0;
        }
    }
}
