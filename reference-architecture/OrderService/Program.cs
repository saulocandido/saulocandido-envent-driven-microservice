using Common.CQRS.Abstration.command;
using Common.CQRS.Abstration.Queries;
using Common.eventDriven.Abistraction.MessageBus;
using Common.EventDriven.Abstraction.MessageBus;
using Common.EventDriven.Interfaces;
using Common.Integration.Events;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Integration.EventHandlers;
using OrderService.Repositories;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// add abstraction CQRS
builder.Services.AddScoped<ICommandBroker, CommandBroker>();
builder.Services.AddScoped<IQueryBroker, QueryBroker>();

// Add automapper
builder.Services.AddAutoMapper(typeof(Program));

// Add MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Add behaviors
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Add database settings
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

// Bind the CustomerDatabaseSettings from configuration
builder.Services.Configure<OrderDatabaseSettings>(
    builder.Configuration.GetSection("OrderDatabaseSettings"));

// Add event handlers
builder.Services.AddSingleton<CustomerAddressUpdatedEventHandler>();

// Add event bus
builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection("EventBusOptions"));
builder.Services.AddSingleton<IEventBus, EventBus>();


// Configure MongoDB
ConfigureMongoDb(builder.Services);

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Register the event handler
builder.Services.AddTransient<CustomerAddressUpdatedEventHandler>();


var app = builder.Build();

var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe(
    app.Services.GetRequiredService<CustomerAddressUpdatedEventHandler>(),
    nameof(CustomerAddressUpdated),
    "v1"
);




app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

// Use Cloud Events (needed by Dapr)
//app.UseCloudEvents();

app.MapControllers();

// Map Dapr subscriber (needed by Dapr)
//app.MapSubscribeHandler();






app.Run();

void ConfigureMongoDb(IServiceCollection services)
{
    services.AddSingleton<IMongoClient>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<OrderDatabaseSettings>>();
        return new MongoClient(settings.Value.ConnectionString);
    });

    services.AddSingleton<IMongoDatabase>(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        var settings = sp.GetRequiredService<IOptions<OrderDatabaseSettings>>();
        return client.GetDatabase(settings.Value.DatabaseName);
    });

    services.AddSingleton<IMongoCollection<Order>>(sp =>
    {
        var database = sp.GetRequiredService<IMongoDatabase>();
        var settings = sp.GetRequiredService<IOptions<OrderDatabaseSettings>>();
        return database.GetCollection<Order>(settings.Value.CollectionName);
    });
}
