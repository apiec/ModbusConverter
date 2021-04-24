using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModbusConverter.PeripheralDevices;
using ModbusConverter.PeripheralDevices.Config;
using ModbusConverter.PeripheralDevices.Peripherals;

namespace ModbusConverter.Pages
{
    public class PeripheralsModel : PageModel
    {
        public PeripheralsModel(IPeripheralsManager peripheralsManager, IPeripheralsConfigFile configFile)
        {
            PeripheralsManager = peripheralsManager;
            ConfigFile = configFile;
        }

        public IPeripheralsManager PeripheralsManager { get; }
        public IPeripheralsConfigFile ConfigFile { get; }

        public string PeripheralsJson { get; set; }

        public IEnumerable<PeripheralConfig> PeripheralConfigs
        {
            get => PeripheralsManager.Peripherals.Select(p => p.GetConfig()).OrderBy(p => p.Name);
        }

        public void OnGet()
        {
        }
    }
}
