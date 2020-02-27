using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using DotMatrix.Cache;
using DotMatrix.Cache.Common;
using DotMatrix.Datatables;
using DotMatrix.DI;
using DotMatrix.Helpers;
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
			InitializeCache("WEB");

			RouteConfig.RegisterRoutes(RouteTable.Routes);
			ModelBinders.Binders.Add(typeof(DataTablesParam), new DataTablesModelBinder());

			CaptchaHelper.Configure();
		}

		protected void Application_End()
		{
			DependencyRegistrar.Deregister();
		}

		private void InitializeCache(string nodeKey)
		{
			DependencyRegistrar.RegisterSingletonComponent<IQueueHubClient>(new QueueHubClient(ConfigurationManager.AppSettings["QueueService_Endpoint"]));
			DependencyRegistrar.RegisterSingletonComponent<IThrottleCache>(new ThrottleCache());
			Task.WaitAll(new[]
			{
				Task.Run(async () => DependencyRegistrar.RegisterSingletonComponent<IGameCache>(await GameCache.BuildCache(nodeKey))),
				Task.Run(async () => DependencyRegistrar.RegisterSingletonComponent<IPixelCache>(await PixelCache.BuildCache(nodeKey))),
			}, TimeSpan.FromMinutes(10));
		}
	}
}
