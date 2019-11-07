using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
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

            var exchange = DAL.Context.Exchanges.Where(x => x.Id == id)
                .Select(x => new { Exchange = x, SignUp = x.SignUps.Where(s => s.UserId == BifSessionData.Id) }).FirstOrDefault();
            
            if (exchange?.Exchange == null) {
                ViewBag.MessageTitle = "Sign Up";
                ViewBag.Message = "The requested Exchange cannot be found.";
                return View("Message");
            }

            if (exchange.Exchange.OpenDate <= DateTime.Now || exchange.Exchange.CloseDate >= DateTime.Now) {
                ViewBag.PageMessageTitleTitle = "Sign Up";
                ViewBag.Message = "The requested Exchange is not currently open for signups.";
                return View("Message");
            }

            if (exchange.SignUp != null) {
                ViewBag.MessageTitle = "Sign Up";
                ViewBag.Message = $"You are already registered for the {exchange.Exchange.Name} Exchange.";
                return View("Message");
            }
            
            if (!BifSessionData.HasProfile) {
                ViewBag.MessageTitle = "Sign Up";
                ViewBag.Message = $"You have not yet completed your Profile.  Please update your <a href=\"{Url.Action("", "Profile")}\">Profile</a> before signing up.";
                return View("Message");
            }

            SignUp signup = new SignUp {
                ExchangeId = id,
                UserId = BifSessionData.Id,
                SignUpDate = DateTime.Now,
                Approved = false
            };

            DAL.Context.SignUps.Add(signup);
            DAL.Context.SaveChanges();

            return RedirectToAction("", "Home");
        }



    }

}