using Microservice.Bill.Api.StateMachines;
using Microsoft.AspNetCore.SignalR;

namespace Microservice.Bill.Api.SignalR
{
    public class MessageHub : Hub
    {
        public async Task SendStatusAsync(PurchaseState status)
        {
            if (Clients != null)
            {
                await Clients.All
                       .SendAsync("ReceivePurchaseStatus", status);
            }
        }
    }
}
