using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Windows.Forms;
using BiF.DAL.Concrete;
using BiF.DAL.Models;
using BiF.Web.ViewModels.Exchanges;

namespace BiF.Web.Controllers
{
    public class ExchangesController : BaseController
    {

        // GET: Exchanges
        public ActionResult Index() {

            IQueryable<Exchange> exchanges = DAL.Context.Exchanges.Where(x => !x.Deleted);

            IndexVM vm = new IndexVM {
                Exchanges = exchanges.Select(x => new ExchangeVM {
                    Id = x.Id,
                    Name = x.Name,
                    OpenDate = x.OpenDate,
                    CloseDate = x.CloseDate,
                    MatchDate = x.MatchDate
                }).ToList()
            };


            return View(vm);
        }

        //[HttpGet]
        //public ActionResult SignUp(int id) {

        //    Exchange exchange = DAL.Context.Exchanges.Find(id);
        //    if (exchange == null)
        //        return View("Error", null); //TODO: Message View Handler

        //    if (exchange.OpenDate <= DateTime.Now || exchange.CloseDate >= DateTime.Now)
        //        return View("Error", null); //TODO: Message View Handler

        //    SignUp signUp = new SignUp {
        //        ExchangeId = id,
        //        UserId = User.Identity.Name,
        //        SignUpDate = DateTime.Now
        //    };

        //    DAL.Context.SignUps.Add(signUp);
        //    DAL.Context.SaveChanges();

        //    return View();
        //}

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public ActionResult Edit(int id = 0) {
            if (id == 0)
                return View();

            var exchange = DAL.Context.Exchanges.Find(id);
            
            return View();
        }


        [HttpGet]
        public ActionResult SignUp(int id = 0) {

            bool success = signUp(id, BifSessionData.Id, out string message);
            
            if (success)
                return RedirectToAction("", "Home");


            ViewBag.MessageTitle = "Sign Up";
            ViewBag.Message = message;
            return View("Message");
            
        }

        [HttpPost]
        public JsonResult SignUpAsync(string userId, int exchangeId = 0) {
            bool success = signUp(exchangeId, userId, out string message);
            return Json(new {Success = success, Message = message});

        }

        private bool signUp(int exchangeId, string userId, out string message) {

            var exchange = DAL.Context.Exchanges.Where(x => x.Id == exchangeId)
                .Select(x => new {Exchange = x, SignUp = x.SignUps.Any(s => s.UserId == userId)}).FirstOrDefault();

            message = null;
            if (exchange?.Exchange == null) {
                message = "The requested Exchange cannot be found.";
                return false;
            }

            if (exchange.Exchange.OpenDate <= DateTime.Now || exchange.Exchange.CloseDate >= DateTime.Now && !BifSessionData.IsInRole("ADMIN")) { 
                message = "The requested Exchange is not currently open for signups.";
                return false;
            }

            if (exchange.SignUp) {
                message = $"You are already registered for the {exchange.Exchange.Name} Exchange.";
                return false;
            }

            if (!BifSessionData.HasProfile) {
                message = $"You have not yet completed your Profile.  Please update your <a href=\"{Url.Action("", "Profile")}\">Profile</a> before signing up.";
                return false;
            }

            SignUp signup = new SignUp {
                ExchangeId = exchangeId,
                UserId = userId,
                SignUpDate = DateTime.Now,
                Approved = false
            };

            DAL.Context.SignUps.Add(signup);
            DAL.Context.SaveChanges();

            return true;

        }

        public ActionResult Assign(int id) {
            return null;
        }

        public ActionResult ViewStatus(int id)
        {
            return null;
        }

    }

}