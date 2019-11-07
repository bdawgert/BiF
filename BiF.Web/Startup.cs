using Microsoft.Owin;
using Owin;

//[assembly: OwinStartupAttribute(typeof(BiF.Web.Startup))]
namespace BiF.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
