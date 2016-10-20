using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sunny_House.Startup))]
namespace Sunny_House
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
