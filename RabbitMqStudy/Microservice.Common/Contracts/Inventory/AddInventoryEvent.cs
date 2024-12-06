namespace Microservice.Common.Contracts.Inventory
{
    public record AddInventoryEvent:IntegrationEvent
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid CorrelationId { get; set; }
        public long? FormId { get; set; }
        public string? FormType { get; set; }
    }
}
