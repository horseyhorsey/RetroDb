using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace RetroDb.Api.Hubs
{
    public class RocketLauncherHub : Hub
    {
        public RocketLauncherHub()
        {

        }

        public Task SendMessage(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            await this.Clients.All.SendAsync("Send", $"{this.Context.ConnectionId} joined");
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
