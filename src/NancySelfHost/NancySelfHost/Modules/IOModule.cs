using Microsoft.AspNet.SignalR;
using Nancy;
using RaspberryIO;

namespace NancySelfHost.Modules
{
    class IOModule : NancyModule
    {
        public IOModule(Raspberry raspberry)
        {
            Get["/r/speed/{speed:int}"] = parameters =>
            {
                var speed = parameters["speed"];

                raspberry.SetSpeed(speed);

                return speed;
            };

            Get["/r/mode/single/{ledIndex:int}"] = parameters =>
            {
                var ledIndex = parameters["ledIndex"];

                raspberry.ActiveLedIndex = ledIndex;
                raspberry.Mode = LedMode.Single;

                return true;
            };

            Get["/r/mode/moving/{speed:int}"] = parameters =>
            {
                var speed = parameters["speed"];

                raspberry.SetSpeed(speed);
                raspberry.Mode = LedMode.Moving;

                return true;
            };

            raspberry.ActiveLedChanged += raspberry_ActiveLedChanged;
        }

        private void raspberry_ActiveLedChanged(object sender, int e)
        {
            var ctx = GlobalHost.ConnectionManager.GetHubContext("IOHub");
            ctx.Clients.All.indexChanged(e);
        }
    }
}