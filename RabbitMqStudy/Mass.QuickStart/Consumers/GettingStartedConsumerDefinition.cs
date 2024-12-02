﻿using MassTransit;

namespace Mass.QuickStart.Consumers
{
    /// <summary>
    /// 定义消费者
    /// </summary>
    public class GettingStartedConsumerDefinition:ConsumerDefinition<GettingStartedConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GettingStartedConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));

            endpointConfigurator.UseInMemoryOutbox(context);
        }
    }
}
