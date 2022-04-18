using MassTransit;
using MassTransitRider.Contracts;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMassTransit(mt =>
{
    mt.UsingAzureServiceBus((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("ServiceBus"));
        configurator.Message<OrderCreated>(x => x.SetEntityName("order-created"));
    });

    mt.AddRider(rider =>
    {
        rider.UsingEventHub((context, configurator) =>
        {
            configurator.Host(builder.Configuration.GetConnectionString("EventHub"));
            configurator.Storage(builder.Configuration.GetConnectionString("EventHub"));
        });
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
