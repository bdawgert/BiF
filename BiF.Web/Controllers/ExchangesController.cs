using System;
using System.Web.Mvc;
using BiF.DAL.Models;
using BiF.Web.ViewModels.Exchanges;

namespace BiF.Web.Controllers
{
    public class ExchangesController : BaseController
    {



        // GET: Exchanges
        public ActionResult Index() { 
            return View();
        }

        [HttpGet]
        public ActionResult SignUp(int id) {

            Exchange exchange = DAL.Context.Exchanges.Find(id);
            if (exchange == null)
                return View("Error", null); //TODO: Message View Handler

            if (exchange.OpenDate <= DateTime.Now || exchange.CloseDate >= DateTime.Now)
                return View("Error", null); //TODO: Message View Handler



            return View();
        }

        [HttpPost]
        public ActionResult SignUp(SignUpVM vm)
        {





            return RedirectToAction("Index", "Home");
        }

    }
}