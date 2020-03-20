using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookShop2020.Startup))]
namespace BookShop2020
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
