using System.Threading.Tasks;

namespace BatatinhaSignalR.Hubs
{
    public interface IReceiveMessage
    {
        Task ReceiveMessage(string user, string message);

        Task MissingData(string error);
    }
}