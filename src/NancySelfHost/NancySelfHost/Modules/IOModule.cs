using Microsoft.AspNet.SignalR;
using Nancy;
using RaspberryIO;

namespace NancySelfHost.Modules
{
    public class IOModule : NancyModule
    {
        public IOModule(IRaspberry raspberry)
        {
            Get["/r/temperature"] = parameters => raspberry.Temperature.ToString();

            Get["/r/speed/{speed:int}"] = parameters =>
            {
                var speed = parameters["speed"];

                //raspberry.SetSpeed(speed);

                return speed;
            };

            Get["/r/mode/single/{ledIndex:int}"] = parameters =>
            {
                var ledIndex = parameters["ledIndex"];

                raspberry.ActiveLedIndex = ledIndex;
                //raspberry.Mode = LedMode.Single;

                return true;
            };

            Get["/r/mode/moving/{speed:int}"] = parameters =>
            {
                var speed = parameters["speed"];

                //raspberry.SetSpeed(speed);
                //raspberry.Mode = LedMode.Moving;

                return true;
            };

            raspberry.ActiveLedChanged += raspberry_ActiveLedChanged;
            raspberry.TemperatureChanged += raspberry_TemperatureChanged;
        }

        void raspberry_TemperatureChanged(object sender, double e)
        {
            var ctx = GlobalHost.ConnectionManager.GetHubContext("IOHub");
            ctx.Clients.All.temperatureChanged(e);
        }

        private void raspberry_ActiveLedChanged(object sender, int e)
        {
            var ctx = GlobalHost.ConnectionManager.GetHubContext("IOHub");
            ctx.Clients.All.indexChanged(e);
        }
    }
}