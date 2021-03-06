using MassTransit;
using MassTransitRider.Contracts;
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

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog(Log.Logger);

builder.Services.AddControllers();
builder.Services.AddMassTransit(mt =>
{
    mt.UsingAzureServiceBus((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("ServiceBus"));
        // configurator.ConfigureEndpoints(context);
        configurator.Message<OrderCreated>(x=>x.SetEntityName("order-created"));
    });
    
    mt.AddRider(rider =>
    {
        rider.UsingEventHub((context, configurator) =>
        {
            configurator.Host(builder.Configuration.GetConnectionString("EventHub"));
            configurator.Storage(builder.Configuration.GetConnectionString("StorageAccount"));
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
