using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CCU_Crawler.Startup))]
namespace CCU_Crawler
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
