using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GalleriaSupermarket.Startup))]
namespace GalleriaSupermarket
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
