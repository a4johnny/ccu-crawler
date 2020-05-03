using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(阿災.Startup))]
namespace 阿災
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
