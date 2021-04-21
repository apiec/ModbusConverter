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
            //services.AddSingleton<ModbusServer>();
            //services.AddSingleton<ModbusServerWrapper>();

            //services.AddSingleton<GpioController>();


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
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

        }
    }
}
