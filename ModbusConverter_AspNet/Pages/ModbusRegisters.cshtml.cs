using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModbusConverter.Modbus;

namespace ModbusConverter.Pages
{
    public class ModbusRegistersModel : PageModel
    {
        private readonly IModbusServerWrapper _modbusServerWrapper;

        public ModbusRegistersModel(IModbusServerWrapper modbusServerWrapper)
        {
            _modbusServerWrapper = modbusServerWrapper;
        }

        public class RegisterRow
        {
            public int? Address { get; set; }
            public bool? CoilValue { get; set; }
            public bool? DiscreteInputValue { get; set; }
            public int? InputRegisterValue { get; set; }
            public int? HoldingRegisterValue { get; set; }
        }

        [BindProperty] public int StartingAddress { get; set; }
        [BindProperty] public int NumberOfRowsToDisplay { get; set; }

        public List<RegisterRow> Rows
        {
            get
            {
                var coils = _modbusServerWrapper.ReadCoils(StartingAddress, NumberOfRowsToDisplay);
                var discreteInputs = _modbusServerWrapper.ReadDiscreteInputs(StartingAddress, NumberOfRowsToDisplay);
                var inputRegisters = _modbusServerWrapper.ReadInputRegisters(StartingAddress, NumberOfRowsToDisplay);
                var holdingRegisters = _modbusServerWrapper.ReadHoldingRegisters(StartingAddress, NumberOfRowsToDisplay);

                var result = new List<RegisterRow>();
                for (int i = 0; i < NumberOfRowsToDisplay; ++i)
                {
                    result.Add(new RegisterRow
                    {
                        Address = i + StartingAddress,
                        CoilValue = coils[i],
                        DiscreteInputValue = discreteInputs[i],
                        InputRegisterValue = inputRegisters[i],
                        HoldingRegisterValue = holdingRegisters[i]
                    });
                }

                return Rows;
            }
        }


        public void OnGet()
        {
            StartingAddress = 0;
            NumberOfRowsToDisplay = 100;
        }
    }
}
