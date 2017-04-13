using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DotMatrix.Startup))]
namespace DotMatrix
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
