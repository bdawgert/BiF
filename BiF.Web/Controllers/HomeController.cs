using System.Web.Mvc;

namespace BiF.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index() {
            return View();
        }

        public ActionResult Contact() {
            return View();
        }
    }
}