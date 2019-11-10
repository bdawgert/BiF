using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BiF.DAL.Models;
using BiF.Web.ViewModels.Administration;

namespace BiF.Web.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdministrationController : BaseController
    {
        // GET: Administration
        public ActionResult Index() {

            var users = DAL.Context.Users.Select(x => new {User = x, Profile = x.Profile, Roles = x.Roles.Select(s => s.Name)}).ToList();


            List<UserInformation> list = users.Select(x => new UserInformation {
                Id = x.User.Id,
                Email = x.User.Email,
                HasProfile = x.Profile != null,
                Roles = x.Roles.ToArray(),
                UserStatus = (int?)x.User.UserStatus ?? 0

            }).ToList();

            IndexVM vm = new IndexVM {
                Users = list

            };

            return View(vm);
        }


        public JsonResult UpdateRoles(string id, string role, string action) {
            role = role?.ToUpper();
            if (action == "Add" && !DAL.Context.Roles.Any(x => x.UserId == id && x.Name == role))
                DAL.Context.Roles.Add(new IdentityRole {UserId = id, Name = role});
            else if (action == "Remove") {
                IQueryable<IdentityRole> roles = DAL.Context.Roles.Where(x => x.UserId == id && x.Name == role);
                if (roles.Any())
                    DAL.Context.Roles.RemoveRange(roles);
            }
            DAL.Context.SaveChanges();

            return Json(new {Success = true});
        }


        public JsonResult UpdateApproval(string id, int status) {
            IdentityUser user = DAL.Context.Users.Find(id);
            if (user == null)
                return Json(new { Success = false });

            user.UserStatus = (IdentityUser.UserStatuses) status;

            DAL.Context.SaveChanges();

            //createMatchRecord(id, status);

            return Json(new { Success = true });
        }


    }


}