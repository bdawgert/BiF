using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

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

    }
}