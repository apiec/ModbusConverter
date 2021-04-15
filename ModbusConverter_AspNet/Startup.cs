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

namespace ModbusConverter
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddHostedService<AnalogInputsUpdater>();
            //services.AddHostedService<DigitalInputsUpdater>();

            services.AddSingleton<ModbusServer>();

            services.AddSingleton<PeripheralsManager>()
                .AddSingleton<IRegularPeripheralsManager>(x => x.GetRequiredService<PeripheralsManager>())
                .AddSingleton<IOverridingPeripheralsManager>(x => x.GetRequiredService<PeripheralsManager>());

            services.AddSingleton<Peripherals>();
            services.AddSingleton<GpioController>();
            services.AddSingleton<IAnalogInterface, AnalogInterface>();
            services.AddSingleton<IModbusEventsHandler, ModbusEventsHandler>();
            services.AddSingleton<RegistersToPeripheralsMap>();
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

            SubscribeModbusEventsHandler(app);
            StartInputsUpdater(app);
        }

        private void SubscribeModbusEventsHandler(IApplicationBuilder app)
        {
            var eventsHandler = app.ApplicationServices.GetRequiredService<IModbusEventsHandler>();
            var modbusServer = app.ApplicationServices.GetRequiredService<ModbusServer>();

            modbusServer.CoilsChanged += eventsHandler.CoilsChangedHandler;
            modbusServer.HoldingRegistersChanged += eventsHandler.HoldingRegistersChangedHandler;
        }

        private void StartInputsUpdater(IApplicationBuilder app)
        {
            var inputsUpdater = app.ApplicationServices.GetRequiredService<IInputsUpdater>();
            inputsUpdater.Start();
        }

    }
}
