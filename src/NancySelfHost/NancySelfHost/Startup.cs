using Microsoft.AspNet.SignalR;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancySelfHost
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Map the hubs first, otherwise Nancy grabs them and you get a 404.
            var configuration = new HubConfiguration { EnableDetailedErrors = true };
            app.MapSignalR(configuration);
            
            app.UseNancy();
        }
    }
}
