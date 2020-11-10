using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BiF.Web.Utilities;
using BiF.Web.ViewModels.Home;

namespace BiF.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index() {
            
            int exchangeId = BifSessionData.ExchangeId;

            if (Request.IsAuthenticated) {
                Dictionary<int, string> exchanges = DAL.Context.SignUps.Where(x => x.UserId == BifSessionData.Id)
                    .Select(x => new { Id = x.ExchangeId, x.Approved, x.Exchange.Name, x.Exchange.OpenDate })
                    .OrderBy(x => x.OpenDate).ToDictionary(x => x.Id, x => x.Name);

                ViewBag.Exchanges = exchanges;

                if (exchangeId == 0 && exchanges.Any()) {
                    exchangeId = exchanges.Keys.First();
                    BifSessionData.setExchangeId(exchangeId);
                }
            }

            ViewBag.CurrentExchange = exchangeId;

            return View();
        }

        public ActionResult Contact() {
            return View();
        }

        [Authorize]
        public PartialViewResult ParticipantList(int id) {

            List<ParticipantVM> vm = DAL.Context.Users.Where(x => x.Roles.Any(r  => r.Name == "ADMIN"))
                .Select(x => new ParticipantVM {
                    Id = x.Id,
                    UserName = x.Profile.RedditUsername ?? x.Email.Substring(0, x.Email.IndexOf("@", StringComparison.Ordinal)),
                    HasProfile = x.Profile != null,
                    UserStatus = (int) x.UserStatus,
                    IsAdmin = x.Roles.Any(r => r.Name == "ADMIN"),
                    IsSignedUp = x.SignUps.Any(s => s.ExchangeId == id)
            }).OrderByDescending(x => x.IsAdmin).ThenByDescending(x => x.UserStatus).ToList();

            return PartialView("__ParticipantList", vm);

        }


    }
}