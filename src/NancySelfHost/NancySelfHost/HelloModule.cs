using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NancySelfHost
{
    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get["/"] = parameters => "Nancy is running ok.";

            Get["/hello"] = parameters => "Hello World!";

            Get["/os"] = parameters => Environment.OSVersion.ToString();
        }
    }
}
