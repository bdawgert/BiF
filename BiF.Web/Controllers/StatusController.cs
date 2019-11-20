using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BiF.Web.Controllers
{
    public class StatusController : BaseController
    {

        public ActionResult Index() {

            ViewBag.PageTitle = "Status";

            int userCount = DAL.Context.Users.Count(x => x.UserStatus >= 0);
            List<string> exchanges = DAL.Context.Exchanges.Select(x => x.Name).ToList();

            ViewBag.Message =
                $"<p>OK.</p><p>{userCount} Users.</p><p>Exchanges: {string.Join("; " + Environment.NewLine, exchanges)}</p>";

            return View("Message");
        }

    }
}