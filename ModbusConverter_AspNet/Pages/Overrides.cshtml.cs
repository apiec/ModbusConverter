using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModbusConverter.Modbus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModbusConverter.Pages
{
    public class OverrideRow
    {
        public Guid Guid { get; set; }
        public int Address { get; set; }
        public string OverrideExpression { get; set; }
        public string DataType { get; set; }
    }

    public class DynamicOverridesRow
    {
        public OverrideRow CoilRow { get; set; }
        public OverrideRow DiscreteInputRow { get; set; }
        public OverrideRow InputRegisterRow { get; set; }
        public OverrideRow HoldingRegisterRow { get; set; }

    }

    public class OverridesModel : PageModel
    {

        public OverridesModel(IOverridesManager overridesManager, IModbusServerWrapper modbusServerWrapper)
        {
            OverridesManager = overridesManager;
            ModbusServerWrapper = modbusServerWrapper;
        }

        public IOverridesManager OverridesManager { get; }
        public IModbusServerWrapper ModbusServerWrapper { get; }
        [BindProperty] public int Address { get; set; }
        [BindProperty] public string Expression { get; set; }
        [BindProperty] public string RegisterType { get; set; }
        [BindProperty] public DataType DataType { get; set; }
        [BindProperty] public Guid Guid { get; set; }

        public List<DynamicOverridesRow> DynamicOverridesRows
        {
            get
            {
                var result = new List<DynamicOverridesRow>();
                var coilOverrides = OverridesManager.CoilsOverrides.OrderBy(p => p.Address).ToArray();
                var discreteOverrides = OverridesManager.DiscreteInputsOverrides.OrderBy(p => p.Address).ToArray();
                var inputOverrides = OverridesManager.InputRegistersOverrides.OrderBy(p => p.Address).ToArray();
                var holdingOverrides = OverridesManager.HoldingRegistersOverrides.OrderBy(p => p.Address).ToArray();

                var lens = new int[] { coilOverrides.Count(), discreteOverrides.Count(), inputOverrides.Count(), holdingOverrides.Count() };

                for (int i = 0; i < lens.Max(); ++i)
                {
                    result.Add(new DynamicOverridesRow
                    {
                        CoilRow = i < coilOverrides.Count()
                        ? new OverrideRow
                        {
                            Guid = coilOverrides[i].Guid,
                            Address = coilOverrides[i].Address,
                            OverrideExpression = coilOverrides[i].OverrideExpression,
                            DataType = coilOverrides[i].DataType.ToString(),
                        }
                        : null,
                        DiscreteInputRow = i < discreteOverrides.Count()
                        ? new OverrideRow
                        {
                            Guid = discreteOverrides[i].Guid,
                            Address = discreteOverrides[i].Address,
                            OverrideExpression = discreteOverrides[i].OverrideExpression,
                            DataType = discreteOverrides[i].DataType.ToString(),
                        }
                        : null,
                        InputRegisterRow = i < inputOverrides.Count()
                        ? new OverrideRow
                        {
                            Guid = inputOverrides[i].Guid,
                            Address = inputOverrides[i].Address,
                            OverrideExpression = inputOverrides[i].OverrideExpression,
                            DataType = inputOverrides[i].DataType.ToString(),
                        }
                        : null,
                        HoldingRegisterRow = i < holdingOverrides.Count()
                        ? new OverrideRow
                        {
                            Guid = holdingOverrides[i].Guid,
                            Address = holdingOverrides[i].Address,
                            OverrideExpression = holdingOverrides[i].OverrideExpression,
                            DataType = holdingOverrides[i].DataType.ToString(),
                        }
                        : null
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


            switch (registerType)
            {
                case ModbusRegisterType.Coil:
                    OverridesManager.AddCoilOverride(Address, Expression);
                    break;
                case ModbusRegisterType.DiscreteInput:
                    OverridesManager.AddDiscreteInputOverride(Address, Expression);
                    break;
                case ModbusRegisterType.InputRegister:
                    OverridesManager.AddInputRegisterOverride(Address, Expression, DataType);
                    break;
                case ModbusRegisterType.HoldingRegister:
                    OverridesManager.AddHoldingRegisterOverride(Address, Expression, DataType);
                    break;
            }
            Address = 0;
            Expression = string.Empty;
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

            switch (registerType)
            {
                case ModbusRegisterType.Coil:
                    OverridesManager.RemoveCoilOverride(Guid);
                    break;
                case ModbusRegisterType.DiscreteInput:
                    OverridesManager.RemoveDiscreteInputOverride(Guid);
                    break;
                case ModbusRegisterType.InputRegister:
                    OverridesManager.RemoveInputRegisterOverride(Guid);
                    break;
                case ModbusRegisterType.HoldingRegister:
                    OverridesManager.RemoveHoldingRegisterOverride(Guid);
                    break;
            }

            Address = 0;
            Expression = string.Empty;
        }
    }
}
