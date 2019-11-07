using System.Web.Mvc;
using BiF.DAL.Concrete;
using BiF.Web.Identity;

namespace BiF.Web.Controllers
{
    public class BaseController : Controller
    {
        protected BifSessionData BifSessionData = new BifSessionData();
        private EFUnitOfWork _dal;

        public EFUnitOfWork DAL {
            get => _dal ?? EFUnitOfWork.Create();
            private set => _dal = value;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            base.OnActionExecuting(filterContext);
            string username = BifSessionData.Username;
            string email = BifSessionData.Email ?? "user_";
            string emailname = email.Contains("@") ? email.Substring(0, email.IndexOf("@")) : email;
            ViewBag.Username = username ?? emailname;
            ViewBag.Session = BifSessionData;
        }

        public ActionResult Error(string message) {
            return View("Error");
        }

    }
}