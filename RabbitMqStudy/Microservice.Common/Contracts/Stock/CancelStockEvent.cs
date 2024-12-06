namespace Microservice.Common.Contracts.Stock
{
    public record CancelStockEvent : IntegrationEvent
    {
        public long StockId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
