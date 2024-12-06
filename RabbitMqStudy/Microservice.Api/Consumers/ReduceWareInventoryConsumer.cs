using MassTransit;
using MediatR;
using Microservice.Common.Contracts.Inventory;
using Microservice.Inventory.Api.Commonds.UpdateWareInventory;

namespace Microservice.Inventory.Api.Consumers
{
    public class ReduceWareInventoryConsumer : IConsumer<ReduceInventoryEvent>
    {
        private readonly ISender _sender;

        public ReduceWareInventoryConsumer(ISender sender)
        {
            _sender = sender;
        }

        public async Task Consume(ConsumeContext<ReduceInventoryEvent> context)
        {
            var model = new UpdateWareInventoryModel
            (
                context.Message.ProductId,
                -context.Message.Quantity
            );

            var commond = new UpdateWareInventoryCommand(model);

            await _sender.Send(commond);
        }
    }
}
