using MassTransit;
using Microservice.Bill.Api.Contracts;

namespace Microservice.Bill.Api
{
    public class Worker : BackgroundService
    {
        private readonly IBus _bus;

        public Worker(IBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // await _bus.Publish(new GettingStartedEvent { Value = $"The time is {DateTimeOffset.Now}" }, stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
