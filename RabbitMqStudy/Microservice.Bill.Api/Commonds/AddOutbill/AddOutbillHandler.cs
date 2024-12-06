using MassTransit;
using Microservice.Bill.Api.Data;
using Microservice.Bill.Api.Entities;
using Microservice.Common.Contracts.Inventory;
using Microservice.Common.CQRS;
using Microservice.Common.Snowflake;

namespace Microservice.Bill.Api.Commonds.AddOutbill
{
    public class AddStockHandler : ICommandHandler<AddOutbillCommand, AddOutbillResult>
    {
        private readonly SnowflakeIdGenerator _snowflake;
        private readonly ApplicationDbContext _context;
        private readonly IBus _bus;

        public AddStockHandler(SnowflakeIdGenerator snowflake, ApplicationDbContext context, IBus bus)
        {
            _snowflake = snowflake;
            _context = context;
            _bus = bus;
        }
        public async Task<AddOutbillResult> Handle(AddOutbillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                DateTimeOffset nowTime = DateTimeOffset.Now;
                var formNumber = "CK" + nowTime.DateTime.ToString("yyyyMMdd") + nowTime.ToUnixTimeSeconds().ToString();
                var outbill = new Outbill
                {
                    Id = _snowflake.NextId(),
                    FormNumber = formNumber,
                    ProductId = request.model.ProductId,
                    Quantity = request.model.Quantity,
                    Status = 0,
                    DirectorId = Guid.NewGuid(),
                    Notes = request.model.Notes,
                    CreateTime = nowTime.DateTime,
                    CreateBy = "admin"
                };

                _context.Outbills.Add(outbill);
                await _context.SaveChangesAsync();

                var reduceInventoryEvent = new ReduceInventoryEvent
                {
                    ProductId = outbill.ProductId,
                    Quantity = outbill.Quantity,
                    CorrelationId = Guid.NewGuid(),
                    FormId = outbill.Id,
                    FormType = "outbill"
                };

                await _bus.Publish(reduceInventoryEvent);

                return new AddOutbillResult(outbill.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new AddOutbillResult(null);
            }
        }
    }
}
