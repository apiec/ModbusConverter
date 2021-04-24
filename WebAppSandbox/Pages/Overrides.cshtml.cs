using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModbusConverter;

namespace WebAppSandbox.Pages
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

        public List<List<string>> Rows
        {
            get
            {
                var result = new List<List<string>>();
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
                    result.Add(new List<string>
                    {
                        i < coilOverrides.Count() ? coilOverrides[i].Key.ToString() : string.Empty,
                        i < coilOverrides.Count() ? coilOverrides[i].Value.ToString() : string.Empty,
                        i < discreteOverrides.Count() ? discreteOverrides[i].Key.ToString() : string.Empty,
                        i < discreteOverrides.Count() ? discreteOverrides[i].Value.ToString() : string.Empty,
                        i < inputOverrides.Count() ? inputOverrides[i].Key.ToString() : string.Empty,
                        i < inputOverrides.Count() ? inputOverrides[i].Value.ToString() : string.Empty,
                        i < holdingOverrides.Count() ? holdingOverrides[i].Key.ToString() : string.Empty,
                        i < holdingOverrides.Count() ? holdingOverrides[i].Value.ToString() : string.Empty,
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
