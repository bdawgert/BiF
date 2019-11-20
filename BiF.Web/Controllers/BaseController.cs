using System.IO;
using System.Web;
using System.Web.Mvc;
using BiF.DAL.Concrete;
using BiF.Web.Identity;
using Microsoft.Ajax.Utilities;

namespace BiF.Web.Controllers
{
    public class BaseController : Controller
    {
        protected BifSessionData BifSessionData = new BifSessionData();
        private EFUnitOfWork _dal;

        public EFUnitOfWork DAL {
            get => _dal ?? EFUnitOfWork.Create(HttpContext.Application["connectionString"].ToString());
            private set => _dal = value;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            base.OnActionExecuting(filterContext);

            DAL.Reset();

            string username = BifSessionData.Username;
            string email = BifSessionData.Email ?? "user_";
            string emailname = email.Contains("@") ? email.Substring(0, email.IndexOf("@")) : email;
            ViewBag.Username = username ?? emailname;
            ViewBag.Session = BifSessionData;
        }

        public ActionResult Error(string message) {
            return View("Error");
        }

        public static string RenderPartialToString(Controller controller, string viewName, object model) {

            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}