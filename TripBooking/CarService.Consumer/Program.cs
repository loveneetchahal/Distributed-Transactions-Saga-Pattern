﻿using CarService.Consumer.Consumer;
using CarService.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Saga.Core.Concrete.Brokers;
using Saga.Core.MessageBrokers.Concrete;
using Saga.Shared.Consumers.Models.Car;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarService.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", true, true);
                config.AddEnvironmentVariables();

                if (args != null)
                    config.AddCommandLine(args);
            })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    var massTransitSettings = hostContext.Configuration.GetSection("MassTransitSettings")
                        .Get<MassTransitSettings>();
                    services.AddSingleton(massTransitSettings);

                    string connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<CarDbContext>(options =>
                            options.UseSqlServer(connectionString));

                    var bus = BusConfiguration.Instance
                            .ConfigureBus(massTransitSettings, (cfg) =>
                            {
                                var context = services.BuildServiceProvider().GetService<CarDbContext>();
                                cfg.ReceiveEndpoint(nameof(CreateCarBookingEventModel), e =>
                                {
                                    e.Consumer(() => new CreateCarBookingConsumer(context));
                                });
                            });

                    bus.StartAsync();

                    Console.WriteLine("Car Booking Consumer Application started...");
                    Console.ReadLine();
                });
        }
    }
}