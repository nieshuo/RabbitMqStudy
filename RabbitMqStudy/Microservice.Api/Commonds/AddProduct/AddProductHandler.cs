using Microservice.Common.CQRS;
using Microservice.Common.Snowflake;
using Microservice.Inventory.Api.Data;
using Microservice.Inventory.Api.Entities;

namespace Microservice.Inventory.Api.Commonds.AddProduct
{
    public class AddProductHandler : ICommandHandler<AddProductCommand, AddProductResult>
    {
        private readonly SnowflakeIdGenerator _snowflake;
        private readonly ApplicationDbContext _context;

        public AddProductHandler(SnowflakeIdGenerator snowflake, ApplicationDbContext context)
        {
            _snowflake = snowflake;
            _context = context;
        }
        public async Task<AddProductResult> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = new Product
                {
                    Id = _snowflake.NextId(),
                    Name = request.model.Name,
                    Description = request.model.Description
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return new AddProductResult(product.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new AddProductResult(0);
            }
        }
    }
}
