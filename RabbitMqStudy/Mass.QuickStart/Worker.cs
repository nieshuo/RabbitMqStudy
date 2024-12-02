using Mass.QuickStart.Contracts;
using MassTransit;

namespace Mass.QuickStart
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
                await _bus.Publish(new GettingStartedEvent { Value = $"The time is {DateTimeOffset.Now}" }, stoppingToken);

                var hello = new UpdateCustomerAddress
                {
                    CommandId = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    CustomerId = Guid.NewGuid().ToString(),
                    HouseNumber = "12343534",
                    Street = "文化路",
                    City = "杭州",
                    State = "浙江",
                    PostalCode = "310000"
                };

                await _bus.Publish(hello, stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
