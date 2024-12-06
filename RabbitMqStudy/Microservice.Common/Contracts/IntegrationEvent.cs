namespace Microservice.Common.Contracts
{
    public record IntegrationEvent
    {
        public Guid Id => Guid.NewGuid();
        public DateTimeOffset OccurredOn => DateTimeOffset.UtcNow;
        public string EventType => GetType().AssemblyQualifiedName;
    }
}
