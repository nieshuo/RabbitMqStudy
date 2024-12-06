using Microservice.Common;
using System.ComponentModel.DataAnnotations;

namespace Microservice.Inventory.Api.Entities
{
    public class WareInventory : IEntity
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool? HaveActive { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
