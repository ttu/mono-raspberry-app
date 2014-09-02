using FlightDataHandler;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using RaspberryIO;
using System;

namespace NancySelfHost
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // TODO: Dependency Injection for SignalR
        public static Lazy<IDataHandler> DataHandler = new Lazy<IDataHandler>(() => new DataHandler());

        //public static Lazy<IDataHandler> DataHandler = new Lazy<IDataHandler>(() => new DataHandlerServiceObject());

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            Console.WriteLine("Application Startup");

            container.Register<IDataHandler>(Bootstrapper.DataHandler.Value);
            try
            {
                container.Register<Raspberry>(new Raspberry());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // This doesn't work for some reason (would need RouteTables.Routes.MapHubs or something...)
            //GlobalHost.DependencyResolver = new TinyIoCDependencyResolver(container);

            // This neither
            //GlobalHost.DependencyResolver.Register(typeof(IDataHandler),() => dh);

            base.ApplicationStartup(container, pipelines);

            Console.WriteLine("Application Startup ready");
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts", @"Scripts"));
            base.ConfigureConventions(nancyConventions);
        }
    }
}