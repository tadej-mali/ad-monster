using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using AdServer.App_Start;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace AdServer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new Container();
            IocConfig.RegisterTypes(container);
            RegisterWebApiControllers(container);

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private static void RegisterWebApiControllers(Container container)
        {
            var services = GlobalConfiguration.Configuration.Services;
            var controllerTypes = services.GetHttpControllerTypeResolver().GetControllerTypes(services.GetAssembliesResolver());

            // register Web API controllers (important! http://bit.ly/1aMbBW0)
            foreach (var controllerType in controllerTypes)
            {
                container.Register(controllerType);
            }
        }
    }
}