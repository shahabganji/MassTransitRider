using MassTransit;
using MassTransitRider.Contracts;
using MassTransitRider.ServiceBusConsumer;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host,services) =>
    {
        services.AddMassTransit(mt =>
        {
            mt.UsingAzureServiceBus(((context, configurator) =>
            {
                configurator.Host(host.Configuration.GetConnectionString("ServiceBus"));
                // configurator.ConfigureEndpoints(context);
            
                configurator.Message<OrderCreated>(x=>x.SetEntityName("order-created"));
                configurator.SubscriptionEndpoint<OrderCreated>("consumer-app",
                    endpointConfigurator => endpointConfigurator.Consumer<OrderCreatedConsumer>(context));
            }));
            mt.AddConsumer<OrderCreatedConsumer>();
            
            mt.AddRider(rider =>
            {
                rider.AddConsumer<OrderCreatedEventHubConsumer>();

                rider.UsingEventHub((context, configurator) =>
                {
                    configurator.Host(host.Configuration.GetConnectionString("EventHub"));
                    configurator.Storage(host.Configuration.GetConnectionString("StorageAccount"));
                    configurator.ReceiveEndpoint("evh-riders", endpointConfigurator =>
                    {
                        endpointConfigurator.ConfigureConsumer<OrderCreatedEventHubConsumer>(context);
                    });
                });
            });
        });
    });

hostBuilder.UseSerilog(Log.Logger);
var host = hostBuilder.Build();

await host.RunAsync();
