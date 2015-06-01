using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PDFreact.Startup))]
namespace PDFreact
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
