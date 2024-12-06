using MassTransit;
using MediatR;
using Microservice.Common.Contracts;
using Microservice.Common.Contracts.Inventory;
using Microservice.Inventory.Api.Commonds.UpdateWareInventory;

namespace Microservice.Inventory.Api.Consumers
{
    public class AddWareInventoryConsumer : IConsumer<AddInventoryEvent>
    {
        private readonly  ISender _sender;

        public AddWareInventoryConsumer(ISender sender)
        {
            _sender = sender;
        }

        public async Task Consume(ConsumeContext<AddInventoryEvent> context)
        {
            var model = new UpdateWareInventoryModel
            (
                context.Message.ProductId,
                context.Message.Quantity
            );

            var commond = new UpdateWareInventoryCommand(model);

            var result = await _sender.Send(commond);

            var itemsGrantedTask = context.Publish(new SagaAddInventoryEvent(context.Message.CorrelationId));
            await Task.WhenAll(itemsGrantedTask);
        }
    }
}
