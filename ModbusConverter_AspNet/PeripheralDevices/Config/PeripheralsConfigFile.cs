using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModbusConverter.PeripheralDevices.Peripherals;
using ModbusConverter.PeripheralDevices;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;

namespace ModbusConverter.PeripheralDevices.Config
{

    public class PeripheralsConfigFile : IPeripheralsConfigFile
    {
        private readonly IPeripheralsFactory _peripheralsFactory;
        private readonly string _peripheralsFileName;
        private readonly Dictionary<string, Type> _typeNameToConfigTypeDict = new Dictionary<string, Type>
        {
            { nameof(InputPin), typeof(InputPinConfig) },
            { nameof(OutputPin), typeof(OutputPinConfig) },
            { nameof(AnalogInputChannel), typeof(AnalogInputChannelConfig) },
            { nameof(AnalogOutputChannel), typeof(AnalogOutputChannelConfig) },
            { nameof(PwmPin), typeof(PwmPinConfig) }
        };

        public PeripheralsConfigFile(
            IConfiguration configuration,
            IPeripheralsFactory peripheralsFactory)
        {
            _peripheralsFactory = peripheralsFactory;
            _peripheralsFileName = configuration.GetValue<string>("PeripheralsConfigFileName");
        }

        public IEnumerable<IPeripheral> ReadConfigFile()
        {
            var json = File.ReadAllText(_peripheralsFileName);

            using var document = JsonDocument.Parse(json);
            var rootArray = document.RootElement;

            var peripherals = rootArray.EnumerateArray()
                .Select(peripheral => DeserializePeripheralFromJsonElement(peripheral))
                .Where(peripheral => peripheral is not null);

            return peripherals;
        }

        public void WriteToConfigFile(IEnumerable<IPeripheral> peripherals)
        {
            var configs = peripherals
                .Select(peripheral => peripheral.GetConfig());

            var json = JsonSerializer.Serialize(configs);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            using var file = File.OpenWrite(_peripheralsFileName);
            file.Write(bytes);
            file.Close();
        }

        private IPeripheral DeserializePeripheralFromJsonElement(JsonElement peripheralElement)
        {
            if (peripheralElement.TryGetProperty("Type", out var typeElement))
            {
                var typeName = typeElement.GetString();
                var type = _typeNameToConfigTypeDict[typeName];

                var peripheralConfig = (PeripheralConfig)JsonSerializer.Deserialize(peripheralElement.GetRawText(), type);
                var peripheral = _peripheralsFactory.CreateFromConfig(peripheralConfig);
                return peripheral;
            }
            else
            {
                return null;
            }
        }
    }
}
