using System.Collections.Generic;
using System.Linq;
using System.Web;
using BiF.DAL.Concrete;
using BiF.DAL.Models;

namespace BiF.Web.Identity
{
    public class BifSessionData
    {

        public BifSessionData(string id = null) {
            EFUnitOfWork dal = EFUnitOfWork.Create();

            if (id == null)
                id = HttpContext.Current.User.Identity.Name ?? "";

            var user = dal.Context.Users.Where(x => x.Id == id)
                .Select(x => new {Identity = x, Profile = x.Profile, Claims = x.Claims, Roles = x.Roles})
                .FirstOrDefault();

            Id = id;
            Username = user?.Identity.UserName;
            Email = user?.Identity.Email;
            HasProfile = user?.Profile != null;
            Claims = user?.Identity.Claims.ToList();
            Roles = user?.Identity.Roles.ToList();
            Approved = user?.Identity.Approved ?? false;
        }

        public bool IsValid { get; set; }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool HasProfile { get; set; }
        public bool Approved { get; set; }
        public List<IdentityClaim> Claims { get; set; }
        public List<IdentityRole> Roles { get; set; }

        public bool IsInRole(string role) => Roles.Any(x => x.Name == role);
    }
}