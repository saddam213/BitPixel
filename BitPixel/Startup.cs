using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BitPixel.Startup))]
namespace BitPixel
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}
