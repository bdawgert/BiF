using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiF.DAL.Models;
using BiF.Web.Utilities;
using BiF.Web.ViewModels.Exchanges;
using BiF.Web.ViewModels.Home;

namespace BiF.Web.Controllers
{
    [Authorize]
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
        public ActionResult Add()
        {
            return View("Edit");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public ActionResult Edit(string id) {

            int.TryParse(id, out int exchangeId);

            if (exchangeId == 0)
                return View(new EditVM());

            Exchange exchange = DAL.Context.Exchanges.Find(exchangeId);

            if (exchange == null) {
                ViewBag.MessageTitle = "Exchange Not Found";
                ViewBag.Message = $"Exchange #{exchangeId} could not be bound";
                return View("Message");
            }
            
            EditVM vm = new EditVM {
                Id = exchangeId,
                Name = exchange.Name,
                Theme = exchange.Theme,
                Description = exchange.Description,
                OpenDate = exchange.OpenDate,
                CloseDate = exchange.CloseDate,
                MatchDate = exchange.MatchDate,
                ShipDate = exchange.ShipDate,
                MinOunces = exchange.MinOunces,
                MinCost = exchange.MinCost,
                MinRating = exchange.MinRating,
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ActionResult Edit(EditVM vm) {

            int id = vm.Id;

            Exchange exchange;
            if (id == 0)
                exchange = DAL.Context.Exchanges.Add(new Exchange {
                    CreatorId = User.Identity.Name,
                    CreateDate = DateTime.Now
                });
            else
                exchange = DAL.Context.Exchanges.Find(id);

            if (exchange == null)
                return Error("Exchange Id Not Found");

            exchange.Name = vm.Name;
            exchange.Theme = vm.Theme;
            exchange.Description = vm.Description;
            exchange.OpenDate = vm.OpenDate;
            exchange.CloseDate = vm.CloseDate;
            exchange.MatchDate = vm.MatchDate;
            exchange.ShipDate = vm.ShipDate;

            exchange.UpdateDate = DateTime.Now;
            exchange.UpdaterId = User.Identity.Name;
            exchange.MinOunces = vm.MinOunces;
            exchange.MinCost = vm.MinCost;
            exchange.MinRating = vm.MinRating;

            DAL.Context.SaveChanges();

            return RedirectToAction("", "Administration");
        }

        [HttpGet]
        public ActionResult SignUp(int id = 0) {

            var exchange = DAL.Context.Exchanges.Where(x => x.Id == id)
                .Select(x => new { Exchange = x, SignUpDate = (DateTime?)x.SignUps.FirstOrDefault(s => s.UserId == BifSessionData.Id).SignUpDate }).FirstOrDefault();

            if (exchange?.Exchange == null || DateTime.Now < exchange.Exchange.OpenDate) {
                ViewBag.Message = "This exchange is not yet open";
                return View("Message");
            }

            if (DateTime.Now >= exchange.Exchange.MatchDate) {
                ViewBag.Message = $"The {exchange.Exchange.Name} exchange is closed to new Sign Ups";
                return View("Message");
            }

            if (DateTime.Now > exchange.Exchange.CloseDate.AddDays(1)) {
                ViewBag.Message = $"The {exchange.Exchange.Name} exchange is now closed";
                return View("Message");
            }

            SignUpVM vm = new SignUpVM {
                ExchangeId = id,
                Name = exchange.Exchange.Name,
                Description = exchange.Exchange.Description,
                MinCost = exchange.Exchange.MinCost,
                MinOunces = exchange.Exchange.MinOunces,
                MinRating = exchange.Exchange.MinRating,
                OpenDate = exchange.Exchange.OpenDate,
                ShipDate = exchange.Exchange.ShipDate,
                MatchDate = exchange.Exchange.MatchDate,
                CloseDate = exchange.Exchange.CloseDate,
                SignUpDate = exchange.SignUpDate,
                IsAcknowledged = exchange.SignUpDate != null
            };
            
            return View("SignUp", vm);
        }

        [HttpPost]
        public ActionResult SignUp(SignUpVM vm) {

            if (!vm.IsAcknowledged) {
                ModelState.AddModelError("IsAcknowledged", "Please acknowledge the \"Totally Binding Commitment\".");
                return SignUp(vm.ExchangeId);
            }
            
            var exchange = DAL.Context.Exchanges.Where(x => x.Id == vm.ExchangeId)
                .Select(x => new { Exchange = x, IsSignedUp = x.SignUps.Any(s => s.ExchangeId == vm.ExchangeId && s.UserId == BifSessionData.Id) }).FirstOrDefault();

            ViewBag.MessageTitle = "Sign Up";
            if (exchange?.Exchange == null || DateTime.Now < exchange.Exchange.OpenDate) {
                ViewBag.Message = "This exchange is not yet open";
                return View("Message");
            }

            if (DateTime.Now >= exchange.Exchange.MatchDate) {
                ViewBag.Message = "This exchange is closed to new Sign Ups";
                return View("Message");
            }

            if (DateTime.Now > exchange.Exchange.CloseDate.AddDays(1)) {
                ViewBag.Message = "This exchange is now closed";
                return View("Message");
            }

            bool success = doSignUp(exchange.Exchange, BifSessionData.Id, out string message);

            if (!success) {
                ViewBag.Message = message;
                return View("Message");
            }

            CookieManager.SetCookie(new HttpCookie("exchangeId", exchange.Exchange.Id.ToString()));

            return RedirectToAction("", "");
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnSignUp(string exchangeId) {

            int.TryParse(exchangeId, out int id);

            var signUp = DAL.Context.SignUps.Where(x => x.ExchangeId == id && x.UserId == BifSessionData.Id)
                .Select(x => new { SignUp = x, Exchange = x.Exchange}).FirstOrDefault();

            if (signUp?.SignUp == null) {
                return RedirectToAction("", "");
            }

            if (DateTime.Now < signUp.Exchange.MatchDate) {
                DAL.Context.SignUps.Remove(signUp.SignUp);
                DAL.Context.SaveChanges();
                //ViewBag.Title = "";
                //ViewBag.Message = message;
                //return View("Message");
            }

            return RedirectToAction("", "");

        }

        //[HttpPost]
            //public JsonResult SignUpAsync(string userId, int exchangeId = 0) {
            //    bool success = signUp(exchangeId, userId, out string message);
            //    return Json(new {Success = success, Message = message});

            //}

        private bool doSignUp(Exchange exchange, string userId, out string message) {

            message = null;
            if (exchange == null) {
                message = "The requested Exchange cannot be found.";
                return false;
            }

            DateTime signUpEndDate = exchange.MatchDate ?? exchange.OpenDate.AddDays(14);
            if ((DateTime.Now < exchange.OpenDate  || DateTime.Now >= signUpEndDate) && !BifSessionData.IsInRole("ADMIN")) { 
                message = "The requested Exchange is not currently open for signups.";
                return false;
            }

            if (!BifSessionData.HasProfile) {
                message = $"You have not yet completed your Profile.  Please update your <a href=\"{Url.Action("", "Profile")}\">Profile</a> before signing up.";
                return false;
            }

            SignUp signup = new SignUp {
                ExchangeId = exchange.Id,
                UserId = userId,
                SignUpDate = DateTime.Now,
                Approved = false
            };

            DAL.Context.SignUps.Add(signup);
            DAL.Context.SaveChanges();

            return true;

        }

        [Authorize(Roles = "ADMIN")]
        public ActionResult Assign(string id) {

            int.TryParse(id, out int exchangeId);
            
            IEnumerable<Assignment> matches = DAL.Context.Exchanges.Where(x => x.Id == exchangeId).Select(x =>
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
        public PartialViewResult AssignList(string id, string userId) {

            int.TryParse(id, out int exchangeId);

            var selectedUser = DAL.Context.Users.Where(x => x.Id == userId).Select(x => new {
                Username = x.Profile.RedditUsername,
                MatchId = x.SendingMatches.FirstOrDefault(m => m.ExchangeId == BifSessionData.ExchangeId).RecipientId
            }).FirstOrDefault();

            List<SelectListItem> availableUsers = DAL.Context.SignUps.Where(x => x.UserId != userId &&
                     x.ExchangeId == exchangeId && 
                     x.User.UserStatus >= IdentityUser.UserStatuses.None &&
                     !x.User.MatchPreferences.Where(p => p.PreferenceType == MatchPreferenceType.NotUser).Select(p => p.Value).Contains(userId) &&
                     !x.User.ReceivingMatches.Any(m => m.ExchangeId == BifSessionData.ExchangeId && m.SenderId != userId))
                .Select(x => new SelectListItem {
                    Value = x.UserId,
                    Text = x.Profile.RedditUsername
                }).OrderBy(x => x.Text).ToList();

            availableUsers.Insert(0, new SelectListItem {Text = ""});

            AssignListVM vm = new AssignListVM {
                MatchId = selectedUser?.MatchId,
                AvailableUsers = availableUsers,
                SenderUsername = selectedUser?.Username,
                SenderId = userId
            };

            return PartialView("__AssignList", vm);

        }

        [Authorize(Roles = "ADMIN")]
        public JsonResult AssignMatch(string senderId, string recipientId) {
            List<Match> matches = DAL.Context.Matches.Where(x => x.ExchangeId == BifSessionData.ExchangeId && (x.SenderId == senderId || x.RecipientId == recipientId)).ToList();

            if (!string.IsNullOrEmpty(recipientId) && matches.Any(x => x.RecipientId == recipientId))
                return Json(new {Success = false, Message = "Recipient is already assigned in another match"});

            Match match = matches.FirstOrDefault(x => x.SenderId == senderId) ?? new Match
                              {ExchangeId = BifSessionData.ExchangeId, SenderId = senderId};

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
        public ActionResult ViewStatus(string id) {

            int.TryParse(id, out int exchangeId);

            List<ShipmentStatus> userStatus = DAL.Context.Matches.Where(x => x.ExchangeId == exchangeId).Select(x => new ShipmentStatus {
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

        public ActionResult ParticipantList()
        {
            ParticipantsVM vm = new ParticipantsVM();

            var participantData  = DAL.Context.Exchanges.Where(x => x.Id == BifSessionData.ExchangeId)
                .Select(x => new {
                    ExchangeName = x.Name,
                    Participants = x.SignUps.Select(s => new {  
                        Id = s.UserId,
                        RedditUsername = s.Profile.RedditUsername,
                        Email = s.User.Email,
                        City = s.Profile.City, 
                        State = s.Profile.State,
                        HasProfile = s.Profile != null,
                        UserStatus = (int)s.User.UserStatus,
                        IsAdmin = s.User.Roles.Any(r => r.Name == "ADMIN"),
                    })
                }).FirstOrDefault();

            if (participantData == null) {
                ViewBag.Message = "Exchange Not Found";
                return View("Message");
            }

            vm.ExchangeName = participantData.ExchangeName;
            vm.Participants = participantData.Participants.Select(x => new ParticipantVM {
                    Id = x.Id,
                    UserName = x.RedditUsername ?? x.Email.Substring(0, x.Email.IndexOf("@", StringComparison.Ordinal)),
                    Location = $"{x.City}, {x.State}",
                    HasProfile = x.HasProfile,
                    UserStatus = x.UserStatus,
                    IsAdmin = x.IsAdmin,
                    IsSignedUp = true
                }).OrderByDescending(x => x.IsAdmin).ThenByDescending(x => x.UserStatus).ToList();

            return View(vm);



        }


    }

}