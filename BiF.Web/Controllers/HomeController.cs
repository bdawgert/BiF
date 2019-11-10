using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using BiF.Web.ViewModels.Home;

namespace BiF.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index() {

            if (Request.IsAuthenticated)
                ViewBag.Exchanges = DAL.Context.SignUps.Where(x => x.UserId == BifSessionData.Id).Select(x => new { Id = x.ExchangeId, x.Approved, x.Exchange.Name }).ToDictionary(x => x.Id, x => x.Name);


            return View();
        }

        public ActionResult Contact() {
            return View();
        }

        [Authorize]
        public PartialViewResult ParticipantList(int id) {

            List<ParticipantVM> vm = DAL.Context.Users.Where(x => x.UserStatus >= 0)
                .Select(x => new ParticipantVM {
                    Id = x.Id,
                    UserName = x.Profile.RedditUsername ?? x.Email.Substring(0, x.Email.IndexOf("@")),
                    HasProfile = x.Profile != null,
                    UserStatus = (int) x.UserStatus,
                    IsAdmin = x.Roles.Any(r => r.Name == "ADMIN"),
                    IsSignedUp = x.SignUps.Any(s => s.ExchangeId == id)
            }).OrderByDescending(x => x.IsAdmin).ThenByDescending(x => x.UserStatus).ToList();

            return PartialView("__ParticipantList", vm);

        }

    }
}