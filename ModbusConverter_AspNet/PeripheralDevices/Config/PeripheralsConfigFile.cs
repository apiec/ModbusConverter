using Microsoft.Extensions.Configuration;
using ModbusConverter.PeripheralDevices.Peripherals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ModbusConverter.PeripheralDevices.Config
{

    public class PeripheralsConfigFile : IPeripheralsConfigFile
    {
        private readonly IPeripheralsFactory _peripheralsFactory;
        private readonly string _peripheralsFileName;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

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
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.WriteIndented = true;
        }

        public IEnumerable<IPeripheral> ReadConfigFile()
        {
            var json = File.ReadAllText(_peripheralsFileName);

            using var document = JsonDocument.Parse(json);
            var rootArray = document.RootElement;

            var peripherals = rootArray.EnumerateArray()
                .Select(peripheral => DeserializePeripheralFromJsonElement(peripheral))
                .Where(peripheral => peripheral is not null)
                .ToArray();

            return peripherals;
        }

        public void WriteToConfigFile(IEnumerable<IPeripheral> peripherals)
        {
            var json = SerializePeripherals(peripherals);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            using var file = File.OpenWrite(_peripheralsFileName);
            file.Write(bytes);
            file.Close();
        }

        public string SerializePeripherals(IEnumerable<IPeripheral> peripherals)
        {
            var configs = peripherals
                .Select(peripheral => peripheral.GetConfig());

            var jsons = configs.Select(config => JsonSerializer.Serialize(config, _typeNameToConfigTypeDict[config.Type], _jsonSerializerOptions));
            var joinedJsons = string.Join(",\n", jsons);
            var json = $"[\n{joinedJsons}\n]";

            return json;
        }

        private IPeripheral DeserializePeripheralFromJsonElement(JsonElement peripheralElement)
        {
            if (peripheralElement.TryGetProperty("Type", out var typeElement))
            {
                var typeName = typeElement.GetString();
                var type = _typeNameToConfigTypeDict[typeName];

                var peripheralConfig = (PeripheralConfig)JsonSerializer.Deserialize(peripheralElement.GetRawText(), type, _jsonSerializerOptions);
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
