using MassTransit;

namespace Microservice.Bill.Api.StateMachines
{
    public class PurchaseState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public Guid DirectorId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset Received { get; set; }
        public decimal? PurchiaseTotal { get; set; }
        public long? FormId { get; set; }
        public string? FormType { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public string? ErrorMessage { get; set; }
        public int Version { get; set; }
    }
}
