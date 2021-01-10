using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Npgsql.EntityFrameworkCore;
using Common.MessageBus;
using Microsoft.EntityFrameworkCore;

namespace Consumer
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
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    var configuration = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json")
                      .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                      .Build();

                    // logger
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
                    services.AddSingleton<Serilog.ILogger>(Log.Logger);

                    // message bus
                    services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
                    services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
                    services.AddTransient<Middlewares.UnitOfWorkMiddleware>();

                    // domain
                    services.AddSingleton<Common.Domain.IDomainEventPublisher>(provider =>
                        new Common.Domain.InMemoryDomainEventPublisher(provider,
                            new Type[] { typeof(DomainEventHandlers) }));

                    // key-value store
                    services.Configure<Common.Persistence.RedisOptions>(configuration.GetSection("Redis"));
                    services.AddSingleton<Common.Persistence.IKeyValueStore, Common.Persistence.RedisKeyValueStore>();

                    // persistence
                    //services.AddDbContext<Persistence.ConsumerDbContext>();
                    //services.AddDbContext<Persistence.ConsumerDbContext>((provider, options) =>
                    //{
                    //    //options.UseInMemoryDatabase("Consumer");
                    //    var configs = provider.GetRequiredService<IConfiguration>();
                    //    options.UseNpgsql(configs.GetConnectionString("Main"), builder => builder
                    //        .MigrationsAssembly(typeof(Persistence.ConsumerDbContext).Assembly.GetName().Name));
                    //});
                    services.AddSingleton<Common.Persistence.IUnitOfWork, Persistence.ConsumerDbContext>();
                    services.AddTransient<Repositories.ICustomerRepository, Repositories.CustomerRepository>();
                    services.AddTransient<Repositories.IOrderRepository, Repositories.OrderRepository>();

                    // application services
                    services.AddHostedService<ConsumerWorker>();
                    services.AddSingleton<ConsumerHandlers>();
                    services.AddSingleton<DomainEventHandlers>();
                    services.AddTransient<DomainServices.IDomainService, DomainServices.DomainService>();
                });
    }
}
