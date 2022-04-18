using MassTransit;
using MassTransitRider.Contracts;
using MassTransitRider.ServiceBusConsumer;
using Microsoft.Extensions.Azure;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host,services) =>
    {
        services.AddMassTransit(mt =>
        {
            mt.UsingAzureServiceBus(((context, configurator) =>
            {
                configurator.Host(host.Configuration.GetConnectionString("ServiceBus"));
            
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
                    configurator.Storage(host.Configuration.GetConnectionString("EventHub"));
                    configurator.ReceiveEndpoint("evh-riders", endpointConfigurator =>
                    {
                        endpointConfigurator.ConfigureConsumer<OrderCreatedEventHubConsumer>(context);
                    });
                });
                
            });
        });
    })
    .Build();

await host.RunAsync();
