using Mass.QuickStart.Contracts;
using MassTransit;

namespace Mass.QuickStart.Consumers
{
    public class GettingStartedConsumer :IConsumer<GettingStartedEvent>
    {
        public readonly ILogger<GettingStartedConsumer> _logger;

        public GettingStartedConsumer(ILogger<GettingStartedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<GettingStartedEvent> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Value);
            return Task.CompletedTask;
        }
    }
}
