namespace Microservice.Common.Contracts
{
    public record SagaAddStockEvent(long StockId,string FormType,Guid CorrelationId);
    public record SagaAddInventoryEvent(Guid CorrelationId);
}
