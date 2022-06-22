using BatatinhaSignalR.HostedService;
using BatatinhaSignalR.Model;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace BatatinhaSignalR.Hubs
{
    public class ChatHub : Hub<IReceiveMessage>
    {
        public ChatHub()
        {
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }
    }
}
