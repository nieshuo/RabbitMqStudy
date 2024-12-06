using Microservice.Bill.Api.Data;
using Microservice.Bill.Api.Entities;
using Microservice.Common.CQRS;
using Microservice.Common.Snowflake;

namespace Microservice.Bill.Api.Commonds.AddStock
{
    public class AddStockHandler : ICommandHandler<AddStockCommand, AddStockResult>
    {
        private readonly SnowflakeIdGenerator _snowflake;
        private readonly ApplicationDbContext _context;

        public AddStockHandler(SnowflakeIdGenerator snowflake, ApplicationDbContext context)
        {
            _snowflake = snowflake;
            _context = context;
        }
        public async Task<AddStockResult> Handle(AddStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                DateTimeOffset nowTime = DateTimeOffset.Now;
                var formNumber = "RK" + nowTime.DateTime.ToString("yyyyMMdd") + nowTime.ToUnixTimeSeconds().ToString();
                var stock = new Stock
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

                _context.Stocks.Add(stock);
                await _context.SaveChangesAsync();

                return new AddStockResult(stock.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new AddStockResult(null);
            }
        }
    }
}
