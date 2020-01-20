using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

using DotMatrix.DI;
using DotMatrix.QueueService.Client;
using DotMatrix.QueueService.Common;

namespace DotMatrix
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			MvcHandler.DisableMvcResponseHeader = true;
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new RazorViewEngine());
			DependencyRegistrar.Register();

			var queueHubClient = new QueueHubClient();
			DependencyRegistrar.RegisterSingletonComponent<IQueueHubClient>(() => queueHubClient);

			GlobalConfiguration.Configure(WebApiConfig.Register);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}
	}
}
