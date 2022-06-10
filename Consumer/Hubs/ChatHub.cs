using BatatinhaSignalR.HostedService;
using BatatinhaSignalR.Model;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace BatatinhaSignalR.Hubs
{
    public class ChatHub : Hub<IReceiveMessage>
    {
        private readonly ICalculator _calculator;
        public ChatHub(ICalculator calculator)
        {
            _calculator = calculator;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.GetValuePrint("Hello");
        }
    }
}
