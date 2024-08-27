using CustomerService.Configuration;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Repositories;
using MediatR;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Reflection;
using Common.EventDriven.Interfaces;
using Common.eventDriven.Abistraction.MessageBus;
using Common.EventDriven.Abstraction.MessageBus;
using Common.CQRS.Abstration.command;
using Common.CQRS.Abstration.Queries;

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

//Bind the CustomerDatabaseSettings from configuration
builder.Services.Configure<CustomerDatabaseSettings>(
    builder.Configuration.GetSection("CustomerDatabaseSettings"));

// Configure event bus
builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection("EventBusOptions"));
builder.Services.AddSingleton<IEventBus, EventBus>();

// Configure MongoDB
ConfigureMongoDb(builder.Services);

// Add CustomerRepository
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

void ConfigureMongoDb(IServiceCollection services)
{
    services.AddSingleton<IMongoClient>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<CustomerDatabaseSettings>>();
        return new MongoClient(settings.Value.ConnectionString);
    });

    services.AddSingleton<IMongoDatabase>(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        var settings = sp.GetRequiredService<IOptions<CustomerDatabaseSettings>>();
        return client.GetDatabase(settings.Value.DatabaseName);
    });

    services.AddSingleton<IMongoCollection<Customer>>(sp =>
    {
        var database = sp.GetRequiredService<IMongoDatabase>();
        var settings = sp.GetRequiredService<IOptions<CustomerDatabaseSettings>>();
        return database.GetCollection<Customer>(settings.Value.CollectionName);
    });
}
