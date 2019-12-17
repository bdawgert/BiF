using System;
using System.Collections.Generic;
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

            //Exchange exchange = DAL.Context.Exchanges.Find(id);
            
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

        [Authorize(Roles = "ADMIN")]
        public ActionResult Assign() {

            IEnumerable<Assignment> matches = DAL.Context.Exchanges.Where(x => x.Id == 2).Select(x =>
                x.SignUps.Where(s => s.User.UserStatus > IdentityUser.UserStatuses.None)
                    .GroupJoin(x.Matches, s => s.UserId, m => m.SenderId,
                        (s, m) => new {
                            SenderId = s.UserId, SenderUsername = s.Profile.RedditUsername, Match = m.FirstOrDefault()
                        }).Select(s => new Assignment {
                        SenderId = s.SenderId, SenderUsername = s.SenderUsername, RecipientId = s.Match.RecipientId,
                        RecipientUsername = s.Match.Recipient.Profile.RedditUsername
                    }).OrderBy(s => s.SenderUsername)
                ).First();

            AssignVM vm = new AssignVM {
                Assignments = matches.ToList()
            };

            return View(vm);
        }


        [Authorize(Roles = "ADMIN")]
        public PartialViewResult AssignList(string id) {

            var selectedUser = DAL.Context.Users.Where(x => x.Id == id).Select(x => new {
                Username = x.Profile.RedditUsername,
                MatchId = x.SendingMatches.FirstOrDefault(m => m.ExchangeId == 2).RecipientId
            }).FirstOrDefault();

            List<SelectListItem> availableUsers = DAL.Context.SignUps.Where(x => x.UserId != id &&
                     x.ExchangeId == 2 && 
                     x.User.UserStatus >= IdentityUser.UserStatuses.None &&
                     !x.User.MatchPreferences.Where(p => p.PreferenceType == MatchPreferenceType.NotUser).Select(p => p.Value).Contains(id) &&
                     !x.User.ReceivingMatches.Any(m => m.ExchangeId == 2 && m.SenderId != id))
                .Select(x => new SelectListItem {
                    Value = x.UserId,
                    Text = x.Profile.RedditUsername
                }).OrderBy(x => x.Text).ToList();

            availableUsers.Insert(0, new SelectListItem {Text = ""});

            AssignListVM vm = new AssignListVM {
                MatchId = selectedUser?.MatchId,
                AvailableUsers = availableUsers,
                SenderUsername = selectedUser?.Username,
                SenderId = id
            };

            return PartialView("__AssignList", vm);

        }

        [Authorize(Roles = "ADMIN")]
        public JsonResult AssignMatch(string senderId, string recipientId) {
            List<Match> matches = DAL.Context.Matches.Where(x => x.ExchangeId == 2 && (x.SenderId == senderId || x.RecipientId == recipientId)).ToList();

            if (!string.IsNullOrEmpty(recipientId) && matches.Any(x => x.RecipientId == recipientId))
                return Json(new {Success = false, Message = "Recipient is already assigned in another match"});

            var match = matches.FirstOrDefault(x => x.SenderId == senderId) ?? new Match
                            {ExchangeId = 2, SenderId = senderId};

            if (match.MatchDate == null)
                DAL.Context.Matches.Add(match);

            if (recipientId == "")
                recipientId = null;

            match.MatchDate = DateTime.Now;
            match.RecipientId = recipientId;

            match.Carrier = null;
            match.TrackingNo = null;
            match.ShipDate = null;

            DAL.Context.SaveChanges();

            return Json(new { Success = true, UserId = senderId, MatchId = recipientId });
        }

        [Authorize(Roles = "ADMIN")]
        public ActionResult ViewStatus(int id = 0) {
            id = 2;

            List<ShipmentStatus> userStatus = DAL.Context.Matches.Where(x => x.ExchangeId == id).Select(x => new ShipmentStatus {
                Sender = x.Sender.Profile.RedditUsername,
                SenderId = x.SenderId,
                Carrier = x.Carrier,
                TrackingNo = x.TrackingNo,
                ShipDate = x.ShipDate,
                Recipient = x.Recipient.Profile.RedditUsername,
            }).OrderBy(x => x.ShipDate != null).ThenBy(x => x.ShipDate).ToList();

            ViewStatusVM vm = new ViewStatusVM {
                ShipmentStatuses = userStatus
            };


            return View(vm);

        }




    }

}