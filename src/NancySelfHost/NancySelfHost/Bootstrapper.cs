﻿using FlightDataHandler;
using FlightDataHandlerService;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using System;

namespace NancySelfHost
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // TODO: Dependency Injection for SignalR
        //public static Lazy<IDataHandler> DataHandler = new Lazy<IDataHandler>(() => new DataHandler());
        public static Lazy<IDataHandler> DataHandler = new Lazy<IDataHandler>(() => new DataHandlerServiceObject());

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.Register<IDataHandler>(Bootstrapper.DataHandler.Value);

            // This doesn't work for some reason (would need RouteTables.Routes.MapHubs or something...)
            //GlobalHost.DependencyResolver = new TinyIoCDependencyResolver(container);

            // This neither
            //GlobalHost.DependencyResolver.Register(typeof(IDataHandler),() => dh);

            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts", @"Scripts"));
            base.ConfigureConventions(nancyConventions);
        }
    }
}