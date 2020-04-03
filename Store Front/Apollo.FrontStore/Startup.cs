using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Apollo.FrontStore.Startup))]
namespace Apollo.FrontStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}