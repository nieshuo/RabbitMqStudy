using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Common.Snowflake
{
    public static class Extensions
    {
        public static IServiceCollection AddSnowflake(
            this IServiceCollection services,IConfiguration configuration)
        {
            var workerId = long.Parse(configuration["SnowflakeSettings:WorkerId"]!);
            var datacenterId = long.Parse(configuration["SnowflakeSettings:DatacenterId"]!);

            services.AddScoped(option => {
                return new SnowflakeIdGenerator(workerId, datacenterId);
            });

            return services;
        }
    }
}
