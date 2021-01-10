using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Common.MessageBus;

namespace Producer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    var configuration = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json")
                      .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                      .Build();

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
                    services.AddSingleton<Serilog.ILogger>(Log.Logger);

                    services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
                    services.AddSingleton<IMessageBus, RabbitMqMessageBus>();

                    services.AddHostedService<ProducerWorker>();
                });
    }
}
