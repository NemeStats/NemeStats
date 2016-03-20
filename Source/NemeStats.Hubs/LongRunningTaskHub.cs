using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace NemeStats.Hubs
{
    public class LongRunningTaskHub : Hub
    {
        public async Task Join(string gamingGroupId)
        {
            await Groups.Add(Context.ConnectionId, gamingGroupId);
        }
    }
}