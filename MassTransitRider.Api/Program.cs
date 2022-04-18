using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMassTransit(mt =>
{
    mt.UsingAzureServiceBus((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("ServiceBus"));
        configurator.ConfigureEndpoints(context);
        // configurator.Message<OrderCreated>(x=>x.SetEntityName("order-created"));
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
