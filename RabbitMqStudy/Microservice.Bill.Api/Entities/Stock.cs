using Microservice.Common;

namespace Microservice.Bill.Api.Entities
{
    public class Stock : IEntity
    {
        public long Id { get; set; }
        public string FormNumber { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public Guid DirectorId { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CancelTime { get; set; }
        public string? CancelBy { get; set; }
    }
}
