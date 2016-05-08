using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace NemeStats.Hubs
{
    public class NotificationsHub : Hub
    {
        public async Task Join(string currentUserId)
        {
            await Groups.Add(Context.ConnectionId, currentUserId);
        }
    }
}