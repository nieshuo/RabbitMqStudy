using Microservice.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microservice.Common.CQRS
{
    public static class Extensions
    {
        public static IServiceCollection AddMediatRServices(
            this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetEntryAssembly());
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
                config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
            });
            return services;
        }
    }
}
