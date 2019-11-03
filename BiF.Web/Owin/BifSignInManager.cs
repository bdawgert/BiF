using System.Security.Claims;
using System.Threading.Tasks;
using BiF.DAL.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace BiF.Web.Owin
{
    public class BifSignInManager : SignInManager<BifIdentityUser, string>
    {
        public BifSignInManager(BifUserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(BifIdentityUser user) {
            return user.GenerateUserIdentityAsync((BifUserManager)UserManager);
        }

        public static BifSignInManager Create(IdentityFactoryOptions<BifSignInManager> options, IOwinContext context) {
            return new BifSignInManager(context.GetUserManager<BifUserManager>(), context.Authentication);
        }
    }
}