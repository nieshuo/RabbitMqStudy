using Microservice.Common;

namespace Microservice.Inventory.Api.Entities
{
    public class Product : IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
