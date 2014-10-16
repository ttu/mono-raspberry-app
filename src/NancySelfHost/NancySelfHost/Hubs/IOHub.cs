using Microsoft.AspNet.SignalR;

namespace NancySelfHost.Hubs
{
    public class IOHub : Hub
    {
        public void GetVersion()
        {
            Clients.Caller.getVersion("1.1.7");
        }
    }
}