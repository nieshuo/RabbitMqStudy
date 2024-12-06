using MassTransit;
using Microservice.Bill.Api.Activities;
using Microservice.Bill.Api.SignalR;
using Microservice.Common.Contracts;
using Microservice.Common.Contracts.Inventory;
using Microservice.Common.Contracts.Purchase;
using Microservice.Common.Contracts.Stock;

namespace Microservice.Bill.Api.StateMachines
{
    public class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
    {
        private readonly MessageHub _messageHub;
        public State Accepted { get; }
        public State AddStockGranted { get; }
        public State UpdateInventoryGranted { get; }
        public State Completed { get; }
        public State Faulted { get; }

        public Event<PurchaseRequested> PurchaseRequested { get; }
        public Event<GetPurchaseState> GetPurchaseState { get; }
        public Event<SagaAddStockEvent> SagaAddStockEvent { get; }
        public Event<SagaAddInventoryEvent> SagaAddInventoryEvent { get; }

        public Event<Fault<AddStockEvent>> AddStockEventFaulted { get; }
        public Event<Fault<AddInventoryEvent>> AddInventoryEventFaulted { get; }

        public PurchaseStateMachine(MessageHub messageHub)
        {
            InstanceState(state => state.CurrentState);
            ConfigureEvents();
            ConfigureInitialState();
            ConfigureAny();
            ConfigureAccepted();
            ConfigureItemGranted();
            ConfigureFaulted();
            ConfigureCompletd();
            _messageHub = messageHub;
        }

        private void ConfigureEvents()
        {
            Event(() => PurchaseRequested);
            Event(() => GetPurchaseState);
            Event(() => SagaAddStockEvent);
            Event(() => SagaAddInventoryEvent);
            Event(() => AddStockEventFaulted, x => x.CorrelateById(context => context.Message.Message.CorrelationId));
            Event(() => AddInventoryEventFaulted, x => x.CorrelateById(context => context.Message.Message.CorrelationId));
        }

        private void ConfigureInitialState()
        {
            Initially(
                When(PurchaseRequested)
                    .Then(context => {
                        context.Saga.DirectorId = context.Message.DirectorId;
                        context.Saga.ProductId = context.Message.ProductId;
                        context.Saga.Quantity = context.Message.Quantity;
                        context.Saga.Received = DateTimeOffset.UtcNow;
                        context.Saga.LastUpdated = context.Saga.Received;
                    })
                    .Activity(x => x.OfType<CalculatePurchaseTotalActivity>())
                    .Publish(context => new AddStockEvent
                    {
                        DirectorId = context.Saga.DirectorId,
                        ProductId = context.Saga.ProductId,
                        Quantity = context.Saga.Quantity,
                        CorrelationId = context.Saga.CorrelationId
                    })
                    .TransitionTo(Accepted)
                    .Catch<Exception>(ex => ex.
                        Then(context =>
                        {
                            context.Saga.ErrorMessage = context.Exception.Message;
                            context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                        })
                        .TransitionTo(Faulted)
                        .ThenAsync(async context => await _messageHub.SendStatusAsync(context.Saga))
                     )
            );
        }

        private void ConfigureAccepted()
        {
            During(Accepted,
                Ignore(PurchaseRequested),
                When(SagaAddStockEvent)
                    .Then(context =>
                    {
                        context.Saga.FormId = context.Message.StockId;
                        context.Saga.FormType = context.Message.FormType;
                        context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                    })
                    .Publish(context => new AddInventoryEvent
                    {
                        ProductId = context.Saga.ProductId,
                        Quantity = context.Saga.Quantity,
                        CorrelationId = context.Saga.CorrelationId,
                        FormType = context.Saga.FormType,
                        FormId = context.Saga.FormId
                    })
                    .TransitionTo(UpdateInventoryGranted),
                When(AddStockEventFaulted)
                    .Then(context =>
                    {
                        context.Saga.ErrorMessage = context.Message.Exceptions[0].Message;
                        context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                    })
                    .TransitionTo(Faulted)
                    .ThenAsync(async context => await _messageHub.SendStatusAsync(context.Saga))
                );
        }

        private void ConfigureItemGranted()
        {
            During(UpdateInventoryGranted,
                Ignore(PurchaseRequested),
                Ignore(SagaAddStockEvent),
                When(SagaAddInventoryEvent)
                    .Then(context =>
                    {
                        context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                    })
                    .TransitionTo(Completed)
                    .ThenAsync(async context => await _messageHub.SendStatusAsync(context.Saga)),
                When(AddInventoryEventFaulted)
                    .Publish(context => new ReduceInventoryEvent{
                        ProductId =context.Saga.ProductId,
                        Quantity = context.Saga.Quantity,
                        CorrelationId = context.Saga.CorrelationId,
                        FormId = context.Saga.FormId,
                        FormType = "cancel stock"
                    })
                    .Then(context =>
                    {
                        context.Saga.ErrorMessage = context.Message.Exceptions[0].Message;
                        context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                    })
                    .TransitionTo(Faulted)
                    .ThenAsync(async context => await _messageHub.SendStatusAsync(context.Saga))
                );
        }

        private void ConfigureCompletd()
        {
            During(Completed,
                Ignore(PurchaseRequested),
                Ignore(SagaAddStockEvent),
                Ignore(SagaAddInventoryEvent));
        }

        private void ConfigureAny()
        {
            DuringAny(
                When(GetPurchaseState)
                .Respond(x => x.Saga)
            );
        }

        private void ConfigureFaulted()
        {
            During(Faulted,
                Ignore(PurchaseRequested),
                Ignore(SagaAddStockEvent),
                Ignore(SagaAddInventoryEvent)
            );
        }
    }
}
