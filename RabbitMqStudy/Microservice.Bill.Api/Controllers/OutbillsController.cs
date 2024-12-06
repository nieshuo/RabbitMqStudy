using MassTransit;
using MediatR;
using Microservice.Bill.Api.Commonds.AddOutbill;
using Microservice.Bill.Api.Entities;
using Microservice.Bill.Api.StateMachines;
using Microservice.Common.Contracts.Purchase;
using Microsoft.AspNetCore.Mvc;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Microservice.Bill.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutbillsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<GetPurchaseState> _purchaseClient;

        public OutbillsController(ISender sender, IPublishEndpoint publishEndpoint, IRequestClient<GetPurchaseState> purchaseClient)
        {
            _sender = sender;
            _publishEndpoint = publishEndpoint;
            _purchaseClient = purchaseClient;
        }

        // GET: api/<OutbillsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var message = new PurchaseRequested{
                DirectorId = Guid.NewGuid(),
                ProductId = 159110164879393,
                Quantity = 10,
                CorrelationId = Guid.NewGuid()
            };

            await _publishEndpoint.Publish(message);
            return Ok(message);
        }

        // GET api/<OutbillsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var response = await _purchaseClient.GetResponse<PurchaseState>(new GetPurchaseState(id));

            var purchaseState = response.Message;
            return Ok(purchaseState);
        }

        // POST api/<OutbillsController>
        [HttpPost]
        public async Task<ActionResult<Outbill>> Post(AddOutbillModel model)
        {
            var commond = new AddOutbillCommand(model);

            var result = await _sender.Send(commond);

            return Ok(result.Id);
        }

        // PUT api/<OutbillsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OutbillsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
