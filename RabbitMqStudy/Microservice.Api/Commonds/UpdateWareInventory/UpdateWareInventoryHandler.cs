using Microservice.Common.CQRS;
using Microservice.Common.Snowflake;
using Microservice.Inventory.Api.Data;
using Microservice.Inventory.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Inventory.Api.Commonds.UpdateWareInventory
{
    public class UpdateWareInventoryHandler : ICommandHandler<UpdateWareInventoryCommand, UpdateWareInventoryResult>
    {
        private readonly SnowflakeIdGenerator _snowflake;
        private readonly ApplicationDbContext _context;

        public UpdateWareInventoryHandler(SnowflakeIdGenerator snowflake, ApplicationDbContext context)
        {
            _snowflake = snowflake;
            _context = context;
        }
        public async Task<UpdateWareInventoryResult> Handle(UpdateWareInventoryCommand request, CancellationToken cancellationToken)
        {
            var wareInventory = await _context.WareInventories.Where(x => x.ProductId == request.model.ProductId).FirstOrDefaultAsync(cancellationToken);

            //_context.Entry<WareInventory>(wareInventory).Property(p => p.RowVersion).OriginalValue = request.model.RowVersion;

            if (wareInventory == null)
            {
                wareInventory = new WareInventory
                {
                    Id = _snowflake.NextId(),
                    ProductId = request.model.ProductId,
                    Quantity = request.model.Quantity,
                    CreateTime = DateTime.UtcNow,
                    HaveActive = true
                };

                _context.WareInventories.Add(wareInventory);
            }
            else
            {
                _context.WareInventories.Attach(wareInventory);
                wareInventory.Quantity += request.model.Quantity;
                wareInventory.UpdateTime = DateTime.UtcNow;

                //_context.WareInventories.Entry(wareInventory).CurrentValues.SetValues(wareInventory);
            }
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateWareInventoryResult(wareInventory.Id);
        }
    }
}
