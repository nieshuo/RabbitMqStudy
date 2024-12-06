using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microservice.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(
            this IServiceCollection services,
            Action<IRetryConfigurator>? configureRetries = null,
            Assembly? assembly = null)
        {
            services.AddMassTransit(configure =>
            {
                configure.SetKebabCaseEndpointNameFormatter();

                // By default, sagas are in-memory, but should be changed to a durable
                // saga repository.
                configure.SetInMemorySagaRepositoryProvider();

                // 设置程序集
                // var entryAssembly = Assembly.GetAssembly(typeof(Program));
                if (assembly == null)
                {
                    assembly = Assembly.GetEntryAssembly();
                }

                configure.AddConsumers(assembly);

                //configure.AddConfigureEndpointsCallback((context, name, cfg) =>
                //{
                //    cfg.UseMessageRetry(r => r.Immediate(5));
                //});

                configure.AddSagaStateMachines(assembly);
                configure.AddSagas(assembly);
                configure.AddActivities(assembly);

                configure.UsingPlayEconomyRabbitMq(configureRetries);
            });

            return services;
        }

        public static void UsingPlayEconomyRabbitMq(
            this IBusRegistrationConfigurator configure,
            Action<IRetryConfigurator>? configureRetries = null)
        {
            configure.UsingRabbitMq((context, cfg) =>
            {
                var configuration = context.GetService<IConfiguration>();

                string host = configuration["RabbitMQSettings:Host"]!;
                ushort port = ushort.Parse(configuration["RabbitMQSettings:Port"]!);
                string virtualHost = configuration["RabbitMQSettings:VirtualHost"]!;
                string userName = configuration["RabbitMQSettings:UserName"]!;
                string password = configuration["RabbitMQSettings:Password"]!;

                cfg.Host(host, port,virtualHost, h => {
                    h.Username(userName);
                    h.Password(password);
                });

                if (configureRetries == null)
                {
                    configureRetries = (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                }

                cfg.UseMessageRetry(configureRetries);

                // 别忘了这个
                cfg.ConfigureEndpoints(context);
            });
        }
    }
}
