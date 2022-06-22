using BatatinhaSignalR.Hubs;
using BatatinhaSignalR.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BatatinhaSignalR.HostedService
{
    public class CalculatorHostedService : BackgroundService
    {
        private readonly ChannelReader<ValueControllerModelOne> _readerOne;
        private readonly ChannelReader<ValueControllerModelTwo> _readerTwo;
        private readonly IHubContext<ChatHub, IReceiveMessage> _clockHub;
        private decimal LastCalculatedValue;

        public CalculatorHostedService(ChannelReader<ValueControllerModelOne> readerOne,
            ChannelReader<ValueControllerModelTwo> readerTwo,
            IHubContext<ChatHub, IReceiveMessage> clockHub
            )
        {
            _readerOne = readerOne;
            _readerTwo = readerTwo;
            _clockHub = clockHub;
            LastCalculatedValue = 0;
        }

        private async Task CalculateAndNotify(decimal one, decimal two)
        {
            var total = one + two;

            if (total != LastCalculatedValue)
            {
                LastCalculatedValue = total;

                var resultObservableModel = new ResultObservableModel { Total = total };

                Console.WriteLine("Novo valor:" + total.ToString());

                await _clockHub.Clients.All.ReceiveMessage("total" , total.ToString());
            }
            else
            {
               await _clockHub.Clients.All.MissingData("sem update");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                var lastOne = new ValueControllerModelOne();
                var lastTwo = new ValueControllerModelTwo();
                var newValue = false;

                while (!stoppingToken.IsCancellationRequested)
                {
                    newValue = false;

                    _readerOne.TryRead(out var valueControllerModelOne);
                    _readerTwo.TryRead(out var valueControllerModelTwo);

                    if (valueControllerModelOne != null)
                    {
                        newValue = true;
                        lastOne = valueControllerModelOne;
                    }

                    if (valueControllerModelTwo != null)
                    {
                        newValue = true;
                        lastTwo = valueControllerModelTwo;
                    }

                    if (newValue && lastOne.Value != 0 && lastTwo.Value != 0)
                        await CalculateAndNotify(lastOne.Value, lastTwo.Value);
                }

            }, stoppingToken);

            return Task.CompletedTask;
        }
    }
}
