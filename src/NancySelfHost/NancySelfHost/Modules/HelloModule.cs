using Nancy;
using RedisDataStorage;
using System;

namespace NancySelfHost
{
    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get["/"] = parameters => "Nancy is running ok.";

            Get["/hello"] = parameters => "Hello World!";

            Get["/os"] = parameters => Environment.OSVersion.ToString();

            Get["/count"] = parameters =>
            {
                var st = new RedisStorageMono();
                var c = st.Get("count");

                if (c == null)
                {
                    c = "1";
                    st.Set("count", c);
                }
                else
                {
                    var integer = System.Convert.ToInt32(c);
                    integer++;

                    c = integer.ToString();
                    st.Set("count", c);
                }

                return c;
            };
        }
    }
}