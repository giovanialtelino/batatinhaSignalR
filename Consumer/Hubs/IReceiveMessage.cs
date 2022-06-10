using System.Threading.Tasks;

namespace BatatinhaSignalR.Hubs
{
    public interface IReceiveMessage
    {
        Task GetValuePrint(string message);
    }
}