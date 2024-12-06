using MassTransit;
using MediatR;
using Microservice.Common.Contracts;
using Microservice.Common.Contracts.Stock;
using Microservice.Bill.Api.Commonds.AddStock;

namespace Microservice.Bill.Api.Consumers
{
    public class AddStockConsumer : IConsumer<AddStockEvent>
    {
        private readonly ISender _sender;

        public AddStockConsumer(ISender sender)
        {
            _sender = sender;
        }

        public async Task Consume(ConsumeContext<AddStockEvent> context)
        {
            var message = context.Message;

            var model = new AddStockModel
            (
                context.Message.ProductId,
                context.Message.Quantity,
                "hello"
            );

            var commond = new AddStockCommand(model);

            var result = await _sender.Send(commond);
            if (result.Id == null)
            {
                throw new Exception("Stock create fail");
            }

            var sagaAddStockEvent = new SagaAddStockEvent(
                result.Id.Value,
                "stock",
                context.Message.CorrelationId
            );

            var itemsGrantedTask = context.Publish(sagaAddStockEvent);
            await Task.WhenAll(itemsGrantedTask);
        }
    }
}
