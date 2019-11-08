using System.Configuration;
using System.Linq;
using System.Web;
using BiF.DAL.Concrete;
using BiF.DAL.Models;

namespace BiF.Web.Identity
{
    public class BifUserStore
    {
        private EFUnitOfWork _dal;
        private static BifUserStore _bifUserStore;

        private BifUserStore() {
            _dal = EFUnitOfWork.Create(HttpContext.Current.Application["connectionString"].ToString());
        }

        public IdentityUser User { get; set; }
        
        public IdentityUser LoadUserByEmail(string email) {
            return User = _dal.Context.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
        }

        public IdentityUser LoadUserById(string id) {
            return User = _dal.Context.Users.Find(id);
        }

        public IdentityUser Add(IdentityUser user) {
            return User = _dal.Context.Users.Add(user);
        }

        public static BifUserStore Create() {
            if (_bifUserStore == null)
                _bifUserStore = new BifUserStore();
            return _bifUserStore;
        }

        public static BifUserStore Recreate() {
            _bifUserStore = new BifUserStore();
            return _bifUserStore;
        }

        public void Update() {
            _dal.Context.SaveChanges();
        }


    }
}