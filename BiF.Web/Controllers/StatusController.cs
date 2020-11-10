using System.Linq;
using System.Web.Mvc;

namespace BiF.Web.Controllers
{
    public class StatusController : BaseController
    {

        public ActionResult Index() {

            ViewBag.PageTitle = "Status";

            var users = DAL.Context.Users.Select(x => new { x.Email, x.Profile.RedditUsername, x.UserStatus }).ToList();
            var exchanges = DAL.Context.Exchanges.Select(x => new { x.Name, x.OpenDate, x.SignUps }).ToList();

            int userCount = users.Count(x => x.UserStatus > 0);
            int pendingCount = users.Count(x => x.UserStatus == 0);

            ViewBag.Message =
                "<p>Status: OK</p>" + 
                $"<p>{userCount} Users ({pendingCount} Pending).</p>" + 
                $"<p>Exchanges: {string.Concat(exchanges.Select(x => $"<div>{x.Name} ({x.OpenDate.ToShortDateString()}): {x.SignUps.Count} signups</div>"))}</p>";

            return View("Message");
        }

    }
}