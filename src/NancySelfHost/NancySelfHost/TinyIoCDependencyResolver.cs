using Microsoft.AspNet.SignalR;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NancySelfHost
{
    // http://stackoverflow.com/questions/13817794/signalr-nancyfx-integration

    public class TinyIoCDependencyResolver : DefaultDependencyResolver
    {
        private readonly TinyIoCContainer m_Container;

        public TinyIoCDependencyResolver(TinyIoCContainer container)
        {
            m_Container = container;
        }

        public override object GetService(Type serviceType)
        {
            if (m_Container.CanResolve(serviceType))
            {
                return m_Container.Resolve(serviceType);
            }
            else
            {
                return base.GetService(serviceType);
            }
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var objects = m_Container.CanResolve(serviceType) ? m_Container.ResolveAll(serviceType) : new object[] { };
            return objects.Concat(base.GetServices(serviceType));
        }
    }
}