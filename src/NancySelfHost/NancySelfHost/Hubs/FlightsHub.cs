using FlightDataHandler;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NancySelfHost
{
    public class FlightsHub : Hub
    {
        private IDataHandler _handler;

        public FlightsHub()
        {
            _handler = Bootstrapper.DataHandler.Value;
        }

        public void GetVersion()
        {
            Clients.Caller.getVersion("1.1.2");
        }

        public void GetId()
        {
            Clients.Caller.getId(Context.ConnectionId);
        }

        public void SetLocation(double latitude, double longitude, double elevation)
        {
            _handler.SetClientLocation(Context.ConnectionId, latitude, longitude, elevation);
        }

        public override Task OnConnected()
        {
            _handler.Subscribe(Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _handler.UnSubscribe(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }
    }
}