using MassTransit;
using MediatR;
using Microservice.Bill.Api.Commonds.AddOutbill;
using Microservice.Bill.Api.Commonds.AddStock;
using Microservice.Bill.Api.StateMachines;
using Microservice.Common.Contracts.Purchase;

namespace Microservice.Bill.Api.Activities
{
    public class CalculatePurchaseTotalActivity : IStateMachineActivity<PurchaseState, PurchaseRequested>
    {
        private readonly ISender _sender;

        public CalculatePurchaseTotalActivity(ISender sender)
        {
            _sender = sender;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<PurchaseState, PurchaseRequested> context, IBehavior<PurchaseState, PurchaseRequested> next)
        {
            var message = context.Message;
            //var item = await _dbContext.Products.FindAsync(message.ProductId);
            //if (item == null)
            //{
            //    throw new UnknownItemException(message.ItemId);
            //}

            context.Saga.PurchiaseTotal = 52.25m * message.Quantity;
            context.Saga.LastUpdated = DateTimeOffset.UtcNow;

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<PurchaseState, PurchaseRequested, TException> context, IBehavior<PurchaseState, PurchaseRequested> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("calculate-purchase-total");
        }
    }
}
