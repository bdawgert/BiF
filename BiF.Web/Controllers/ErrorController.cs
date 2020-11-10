using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using BiF.Web.Utilities;

namespace BiF.Web.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error() {
            ViewBag.MessageTitle = "Ummmm.... Error?";
            ViewBag.Message = "Well, this is embarassing.  We weren't expecting an unexpected error.  Not now.  Not like this.";


            return View("Message");
        }

    }
}