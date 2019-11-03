using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Security;
using BiF.DAL.Concrete;
using BiF.DAL.Models;
using Microsoft.AspNet.Identity.Owin;

namespace BiF.Web.Users
{
    public class SignInManager
    {
        private UserManager _userManager;

        private SignInManager(UserManager userManager) {
            _userManager = userManager;
        }

        public Task<ClaimsIdentity> CreateUserIdentityAsync(User user) {
            return user.GenerateUserIdentityAsync((UserManager)UserManager);
        }

        public async Task<bool> PasswordSignInAsync(string email, SecureString password, bool shouldLockout) {
            User user = await _userManager.FindUserByEmailAsync(email);
            string passwordHash = "_".ToLower();

            if (user.PasswordHash.ToLower() == passwordHash) {
                FormsAuthentication
                return true;
            }

            user.AccessFailedCount++;
            if (_userManager.MaxFailedAccessAttemptsBeforeLockout >= user.AccessFailedCount)
                await _userManager.LockoutUserAsync(user);

            return false;

        }

        public async Task<bool> SignInAsync(User user) {

        }

        public static SignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, BifDbContext context) { 
            return new SignInManager(UserManager.Create());
        }
    }
}