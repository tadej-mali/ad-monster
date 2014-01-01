using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Web.Http.Dependencies;
using AdServer.Data;
using AdServer.Service;
using SimpleInjector;
using SimpleInjector.Integration.Web;

namespace AdServer.App_Start
{
    public class IocConfig
    {
        public static Container Instance { get; private set; }

        public static void RegisterTypes(Container container)
        {
            Instance = container;

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcAttributeFilterProvider();

            container.Register<IDirectoryService, DirectoryService>();
            container.Register<IDbContext, DbContextProxy<AdvertisingContext>>();
            container.Register<AdvertisingContext, AdvertisingContext>(new WebRequestLifestyle(true));
        }
    }

    public sealed class SimpleInjectorWebApiDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly Container container;

        public SimpleInjectorWebApiDependencyResolver(Container container)
        {
            this.container = container;
        }

        [DebuggerStepThrough]
        public IDependencyScope BeginScope()
        {
            return this;
        }

        [DebuggerStepThrough]
        public object GetService(Type serviceType)
        {
            return ((IServiceProvider)this.container).GetService(serviceType);
        }

        [DebuggerStepThrough]
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.container.GetAllInstances(serviceType);
        }

        [DebuggerStepThrough]
        public void Dispose()
        {
        }
    }
}