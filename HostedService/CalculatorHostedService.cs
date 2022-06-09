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
    public class CalculatorHostedService : IHostedService
    {
        private readonly ChannelReader<ValueControllerModelOne> _readerOne;
        private readonly ChannelReader<ValueControllerModelTwo> _readerTwo;
        private readonly IDisposable _subscribers;
        private readonly IHubContext<ChatHub, IReceiveMessage> _clockHub;

        public CalculatorHostedService(ChannelReader<ValueControllerModelOne> readerOne,
            ChannelReader<ValueControllerModelTwo> readerTwo,
            IHubContext<ChatHub, IReceiveMessage> clockHub)
        {
            _readerOne = readerOne;
            _readerTwo = readerTwo;
            _clockHub = clockHub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var lastOne = new ValueControllerModelOne();
            var lastTwo = new ValueControllerModelTwo();
            var newValue = false;

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
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
                        CalculateAndNotify(lastOne.Value, lastTwo.Value);
                }
            });

            return Task.CompletedTask;
        }


        private async Task CalculateAndNotify(decimal one, decimal two)
        {
            var total = one + two;
            var resultObservableModel = new ResultObservableModel { Total = total };

            Console.WriteLine(total.ToString());

            _clockHub.Clients.All.GetValuePrint(resultObservableModel.Total.ToString());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Explodiu Chucks!");
            return Task.CompletedTask;
        }

    }
}
