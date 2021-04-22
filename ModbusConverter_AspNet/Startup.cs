using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using EasyModbus;
using System.Device.I2c;
using ModbusConverter.PeripheralDevices;
using System.Device.Gpio;
using Microsoft.Extensions.Configuration;
using ModbusConverter.PeripheralDevices.Peripherals;
using ModbusConverter.PeripheralDevices.AnalogIO;
using static ModbusConverter.PeripheralDevices.AnalogIO.PCF8591Device;
using System.Text.Json;

namespace ModbusConverter
{
    public class TypeHaver
    {
        public string Type { get; set; }
    }


    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("peripherals.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ModbusServer>();
            services.AddSingleton<ModbusServerWrapper>();
            services.AddSingleton<PeripheralsManager>();
            services.AddSingleton<PeripheralsFactory>();
            services.AddSingleton<PCF8591DeviceFactory>();
            services.AddSingleton<GpioController>();
            services.AddSingleton<IAnalogIOController, AnalogIOController>();

            var peripheralsSection = Configuration.GetSection("Peripherals");

            var peripherals = new List<IPeripheral>();
            foreach (var peripheralSection in peripheralsSection.GetChildren())
            {
                var typeString = peripheralSection.GetValue<string>("Type");
                switch (typeString)
                {
                    case nameof(InputPin):
                        break;
                    case nameof(OutputPin):
                        break;
                    case nameof(AnalogInputChannel):
                        break;
                    case nameof(AnalogOutputChannel):
                        break;
                    case nameof(PwmPin):
                        break;
                }
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            var manager = app.ApplicationServices.GetRequiredService<PeripheralsManager>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var jsons = manager.Peripherals.Select(p =>
                    {
                        switch (p)
                        {
                            case AnalogInputChannel a:
                                return JsonSerializer.Serialize(a);
                            case AnalogOutputChannel a:
                                return JsonSerializer.Serialize(a);
                            case InputPin a:
                                return JsonSerializer.Serialize(a);
                            case OutputPin a:
                                return JsonSerializer.Serialize(a);
                            case PwmPin a:
                                return JsonSerializer.Serialize(a);
                        }

                        return JsonSerializer.Serialize(p);
                    });

                    await context.Response.WriteAsync(string.Join("\n", jsons));
                });
            });

            var server = app.ApplicationServices.GetRequiredService<ModbusServer>();
            server.Listen();

            var factory = app.ApplicationServices.GetRequiredService<PeripheralsFactory>();

            var peripherals = new List<IPeripheral>();

            var pinsList = new List<int>();
            Configuration.GetSection("DigitalPins").Bind(pinsList);

            for (int i = 0; i < pinsList.Count; ++i)
            {
                var pinNumber = pinsList[i];

                IPeripheral pin = i < pinsList.Count / 2
                    ? factory.CreateInputPin(pinNumber)
                    : factory.CreateOutputPin(pinNumber);

                pin.RegisterType = ModbusRegisterType.Coil;
                pin.RegisterAddress = pinNumber;
                peripherals.Add(pin);
            }

            var pwmPins = new List<int>();
            Configuration.GetSection("PWMPins").Bind(pwmPins);
            var gpioController = app.ApplicationServices.GetRequiredService<GpioController>();
            
            for (int i = 0; i < pwmPins.Count; ++i)
            {
                var pwmPin = pwmPins[i];
                var pin = factory.CreatePwmPin(pwmPin);
                pin.RegisterType = ModbusRegisterType.HoldingRegister;
                pin.RegisterAddress = 20 + 4 * i;

                peripherals.Add(pin);
            }

            var pcfCount = Configuration.GetValue<int>("PCF8591Count");
            for (int i = 0; i < pcfCount; ++i)
            {
                var analogInput0_1 = factory.CreateAnalogInputChannel(i, InputMode.Differential_AIN0_AIN1);
                var analogInput2_3 = factory.CreateAnalogInputChannel(i, InputMode.Differential_AIN2_AIN3);
                var analogInput0 = factory.CreateAnalogInputChannel(i, InputMode.SingleEnded_AIN0);
                var analogOutput = factory.CreateAnalogOutputChannel(i);

                analogInput0.RegisterType = ModbusRegisterType.HoldingRegister;
                analogInput0_1.RegisterType = ModbusRegisterType.HoldingRegister;
                analogInput2_3.RegisterType = ModbusRegisterType.HoldingRegister;
                analogOutput.RegisterType = ModbusRegisterType.HoldingRegister;

                analogInput0.RegisterAddress = i * 4 + 1;
                analogInput0_1.RegisterAddress = i * 4 + 2;
                analogInput2_3.RegisterAddress = i * 4 + 3;
                analogOutput.RegisterAddress = i * 4 + 4;

                peripherals.AddRange(new IPeripheral[] { analogInput0_1, analogInput2_3, analogInput0, analogOutput });
            }

            foreach (var peripheral in peripherals)
            {
                manager.AddPeripheral(peripheral);
            }

        }
    }
}
