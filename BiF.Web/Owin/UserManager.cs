using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using BiF.DAL.Concrete;
using BiF.DAL.Models;

namespace BiF.Web.Users
{
    public class UserManager
    {
        private static UserManager _userManager;
        private EFUnitOfWork _dal;

        private UserManager() {
            _dal = EFUnitOfWork.Create();
        }

        public int MaxFailedAccessAttemptsBeforeLockout { get; private set; }

        public async ClaimsIdentity GenerateUserIdentityAsync() {

            _dal.Context.Users.Find();
            return new ClaimsIdentity() { };
        }

        public async Task<User> FindUserByEmailAsync(string email) {
            User user = _dal.Context.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            return user;
        }

        public async Task LockoutUserAsync(User user) {
            user.LockoutEnabled = true;
            _dal.Context.SaveChanges();
        }

        public async bool CreateAsync(User user, SecureString password) {
            User
        }

        public static UserManager Create() {
            if (_userManager == null)
                _userManager = new UserManager();

            _userManager.MaxFailedAccessAttemptsBeforeLockout = 5;

            return _userManager;
        }

    }
}