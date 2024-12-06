namespace Microservice.Common.Contracts.Purchase
{
    public record PurchaseRequested : IntegrationEvent
    {
        public Guid DirectorId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid CorrelationId { get; set; }
    }
    public record GetPurchaseState(Guid CorrelationId);
}
