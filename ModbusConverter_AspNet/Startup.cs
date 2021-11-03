using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using EasyModbus;
using ModbusConverter.PeripheralDevices;
using System.Device.Gpio;
using Microsoft.Extensions.Configuration;
using ModbusConverter.PeripheralDevices.Peripherals;
using ModbusConverter.PeripheralDevices.AnalogIO;
using ModbusConverter.PeripheralDevices.Config;
using ModbusConverter.Modbus;

namespace ModbusConverter
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddSingleton<GpioController>();
            services.AddSingleton<ModbusServer>();

            services.AddSingleton<OverridingModbusServerWrapper>();

            services.AddSingleton<IModbusServerWrapper>(provider => provider.GetRequiredService<OverridingModbusServerWrapper>());
            services.AddSingleton<IOverridesManager>(provider => provider.GetRequiredService<OverridingModbusServerWrapper>());
            
            services.AddSingleton<IPeripheralsManager, PeripheralsManager>();
            services.AddSingleton<IPeripheralsFactory, PeripheralsFactory>();
            services.AddSingleton<IPeripheralsConfigFile, PeripheralsConfigFile>();
            services.AddSingleton<IPCF8591DeviceFactory, PCF8591DeviceFactory>();
            services.AddSingleton<IAnalogIOController, AnalogIOController>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            ModbusServerStartListening(app);
        }

        private static void ModbusServerStartListening(IApplicationBuilder app)
        {
            var server = app.ApplicationServices.GetRequiredService<ModbusServer>();

            server.Listen();
        }
    }
}
