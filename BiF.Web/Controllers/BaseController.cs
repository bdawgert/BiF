using System.Web.Mvc;
using BiF.DAL.Concrete;

namespace BiF.Web.Controllers
{
    public class BaseController : Controller
    {

        protected EFUnitOfWork _dal;

        public EFUnitOfWork DAL {
            get => _dal ?? EFUnitOfWork.Create();
            private set => _dal = value;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Error(string message) {
            return View("Error");
        }

    }
}