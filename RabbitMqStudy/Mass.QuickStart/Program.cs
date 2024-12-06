using Mass.QuickStart;
using Mass.QuickStart.Contracts;
using MassTransit;
using RabbitMQ.Client;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    // By default, sagas are in-memory, but should be changed to a durable
    // saga repository.
    x.SetInMemorySagaRepositoryProvider();

    // ÉèÖÃ³ÌÐò¼¯
    // var entryAssembly = Assembly.GetAssembly(typeof(Program));
    var entryAssembly = Assembly.GetEntryAssembly();

    x.AddConsumers(entryAssembly);

    x.AddConfigureEndpointsCallback((context, name, cfg) =>
    {
        cfg.UseMessageRetry(r => r.Immediate(5));
    });

    x.AddSagaStateMachines(entryAssembly);
    x.AddSagas(entryAssembly);
    x.AddActivities(entryAssembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", 5672, "admin_vhost", h => {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<UpdateCustomerAddress>(x =>
        {
            x.Durable = false; // default: true
            x.AutoDelete = true; // default: false
            x.ExchangeType = ExchangeType.Fanout; // default, allows any valid exchange type
        });

        cfg.ConfigureEndpoints(context);
    });

    //x.UsingInMemory((context, cfg) =>
    //{
    //    cfg.ConfigureEndpoints(context);
    //});
});

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Run();