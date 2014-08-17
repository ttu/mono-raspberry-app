using Nancy;
using Nancy.Conventions;

namespace NancySelfHost
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            // NOTE: Still empty, so this file is not required
            base.ConfigureConventions(nancyConventions);
        }
    }
}