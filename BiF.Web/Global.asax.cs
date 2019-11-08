using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace BiF.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DatabaseConfig.SetConnectionString();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
                return;

            FormsAuthenticationTicket authTicket;
            try {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch {
                return;
            }

            // retrieve roles from UserData
            IEnumerable<Claim> roles = authTicket.UserData.Split(';').Select(x => new Claim(ClaimTypes.Role, x));
            ((ClaimsIdentity)Context.User.Identity).AddClaims(roles);

            Context.User = new ClaimsPrincipal(Context.User.Identity);
        }

    }
}
