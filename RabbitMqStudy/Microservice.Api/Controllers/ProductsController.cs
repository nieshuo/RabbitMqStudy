using MediatR;
using Microservice.Inventory.Api.Commonds.AddProduct;
using Microservice.Inventory.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Inventory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductsController(ISender sender)
        {
            _sender = sender;
        }
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(AddProductModel model)
        {
            var command = new AddProductCommand(model);

            var result = await _sender.Send(command);

            return Ok(result.Id);
            // return CreatedAtAction("GetProduct", new { id = response.Id },response);
        }
    }
}
