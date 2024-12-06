namespace Microservice.Common.Contracts.Stock
{
    public record AddStockEvent : IntegrationEvent
    {
        public Guid DirectorId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
