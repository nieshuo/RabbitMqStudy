using MassTransit;

namespace Mass.QuickStart.Consumers
{
    public class UpdateCustomerAddressConsumerDefinition : ConsumerDefinition<UpdateCustomerAddressConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateCustomerAddressConsumer> consumerConfigurator, IRegistrationContext context)
        {
        }
    }
}
