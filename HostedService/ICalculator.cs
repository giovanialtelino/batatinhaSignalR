using BatatinhaSignalR.Model;
using System;

namespace BatatinhaSignalR.HostedService
{
    public interface ICalculator
    {
        void Publish(ResultObservableModel eventMessage);
        void Subscribe(Action<ResultObservableModel> action);

    }
}
