using Mass.QuickStart.Contracts;
using MassTransit;

namespace Mass.QuickStart.Consumers
{
    public class UpdateCustomerAddressConsumer : IConsumer<UpdateCustomerAddress>
    {
        public readonly ILogger<UpdateCustomerAddressConsumer> _logger;

        public UpdateCustomerAddressConsumer(ILogger<UpdateCustomerAddressConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<UpdateCustomerAddress> context)
        {
            _logger.LogInformation($"Received HouseNumber: {context.Message.HouseNumber}");
            return Task.CompletedTask;
        }
    }
}
