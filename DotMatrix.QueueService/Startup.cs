using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(DotMatrix.QueueService.Startup))]
namespace DotMatrix.QueueService
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			//GlobalHost.HubPipeline.AddModule(new ServiceAuthorizationModule());
			//GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new ServiceHubIdentityProvider());
			app.UseCors(CorsOptions.AllowAll);
			app.MapSignalR(new HubConfiguration
			{
				EnableJavaScriptProxies = false,
				EnableJSONP = false,
#if DEBUG
				EnableDetailedErrors = true,
#else
				EnableDetailedErrors = false,
#endif
			});
		}
	}
}