using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BiF.DAL.Models;
using BiF.Untappd;
using BiF.Untappd.Models.Search;
using BiF.Web.Utilities;
using BiF.Web.ViewModels;
using BiF.Web.ViewModels.Profile;
using Match = BiF.DAL.Models.Match;

namespace BiF.Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string id = null) {

            if (BifSessionData.IsInRole("ADMIN"))
                id = id ?? BifSessionData.Id;
            else
                id = BifSessionData.Id;

            var user = DAL.Context.Users.Where(x => x.Id == id)
                .Select(x => new { Email = x.Email, Profile = x.Profile, IsSignedUp = x.SignUps.Any(s => s.ExchangeId == BifSessionData.ExchangeId)})
                .FirstOrDefault();

            if (user == null)
                return View("Error"); //TODO: Message View

            if (user.Profile == null) 
                return View(new ProfileVM { Email = user.Email });

            string phoneNumber = user.Profile.PhoneNumber?.PadLeft(10, ' ') ?? "";

            ProfileVM vm = new ProfileVM {
                Id = id,
                Name = user.Profile.FullName,
                Address = user.Profile.Address,
                City = user.Profile.City,
                State = user.Profile.State,
                Zip = user.Profile.Zip,
                DeliveryNotes = user.Profile.DeliveryNotes,

                RedditUsername = user.Profile.RedditUsername,
                UntappdUsername = user.Profile.UntappdUsername,

                References = user.Profile.References,
                //Wishlist = user.Profile.Wishlist,
                Comments = user.Profile.Comments,

                Piney = user.Profile.Piney,
                Juicy = user.Profile.Juicy,
                Tart = user.Profile.Tart,
                Funky = user.Profile.Funky,
                Malty = user.Profile.Malty,
                Roasty = user.Profile.Roasty,
                Sweet = user.Profile.Sweet,
                Smokey = user.Profile.Smokey,
                Spicy = user.Profile.Spicy,
                Crisp = user.Profile.Crisp,

                
                Phone = $"{phoneNumber}",
                Email = user.Email,

                UpdateDate = user.Profile.UpdateDate,
                //IsSignedUp = user.IsSignedUp

            };
            
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProfileVM vm) {

            string phoneNumber = vm.Phone;
            if (phoneNumber != null) {
                phoneNumber = Regex.Replace(vm.Phone, @"\D", "");
                if (phoneNumber.Length != 10)
                    ModelState.AddModelError("Phone",
                        "Phone Number is not valid.  Please enter your phone number with area code.  No extensions. (NNN-NNN-NNNN)");
            }

            if (!ModelState.IsValid) {
                vm.Email = BifSessionData.Email;
                return View(vm);
            }

            string id = BifSessionData.IsInRole("ADMIN") ? vm.Id ?? BifSessionData.Id : BifSessionData.Id;

            Profile profile = DAL.Context.Profiles.FirstOrDefault(x => x.Id == id) ?? new Profile {Id = id};

            profile.FullName = vm.Name;
            profile.UntappdUsername = vm.UntappdUsername;
            profile.RedditUsername = vm.RedditUsername;

            profile.Address = vm.Address;
            profile.City = vm.City;
            profile.State = vm.State;
            profile.Zip = vm.Zip;
            profile.PhoneNumber = phoneNumber;
            profile.DeliveryNotes = vm.DeliveryNotes;

            profile.Piney = vm.Piney;
            profile.Juicy = vm.Juicy;
            profile.Tart = vm.Tart;
            profile.Funky = vm.Funky;
            profile.Malty = vm.Malty;
            profile.Roasty = vm.Roasty;
            profile.Sweet = vm.Sweet;
            profile.Smokey = vm.Smokey;
            profile.Spicy = vm.Spicy;
            profile.Crisp = vm.Crisp;

            profile.References = vm.References;
            profile.Comments = vm.Comments;
            //profile.Wishlist = vm.Wishlist;
            profile.UpdateDate = DateTime.UtcNow;

            if (profile.CreateDate == null) {
                profile.CreateDate = DateTime.UtcNow;
                profile.Rating = 4;
                DAL.Context.Profiles.Add(profile);
            }
            DAL.Context.SaveChanges();


            //bool success = signUp(, id, out string message);
            

            if (id == BifSessionData.Id) { // Only send if edited by self
                createUserConfirmationEmail(id);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ViewProfile(string id) {
            var user = DAL.Context.Users.Where(x => x.Id == id).Select(x => new { Email = x.Email, Profile = x.Profile }).FirstOrDefault();

            string phoneNumber = user?.Profile?.PhoneNumber?.PadLeft(10, ' ') ?? "";

            ProfileVM vm = new ProfileVM
            {
                Id = id,
                Name = user.Profile.FullName,
                Address = user.Profile.Address,
                City = user.Profile.City,
                State = user.Profile.State,
                Zip = user.Profile.Zip,
                DeliveryNotes = user.Profile.DeliveryNotes,

                RedditUsername = user.Profile.RedditUsername,
                UntappdUsername = user.Profile.UntappdUsername,

                References = user.Profile.References,
                //Wishlist = user.Profile.Wishlist,
                Comments = user.Profile.Comments,

                Piney = user.Profile.Piney,
                Juicy = user.Profile.Juicy,
                Tart = user.Profile.Tart,
                Funky = user.Profile.Funky,
                Malty = user.Profile.Malty,
                Roasty = user.Profile.Roasty,
                Sweet = user.Profile.Sweet,
                Smokey = user.Profile.Smokey,
                Spicy = user.Profile.Spicy,
                Crisp = user.Profile.Crisp,

                Phone = $"{phoneNumber}",
                Email = user.Email,

                UpdateDate = user.Profile.UpdateDate

            };

            return View("View", vm);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public ActionResult ExchangeList(string id) {
            var user = DAL.Context.Profiles.Where(x => x.Id == id).Select(x => new {
                Id = id, 
                UserName = x.RedditUsername,
                RecieveFromMatches = x.RecipientMatches.Select(s => new { ExchangeId = s.ExchangeId, Id = s.SenderId, Name = s.SenderProfile.RedditUsername }),
                SendToMatches = x.SenderMatches.Select(r => new { ExchangeId = r.ExchangeId, Id = r.RecipientId, Name = r.RecipientProfile.RedditUsername }),
                Exchanges = x.SignUps.Select(s => s.Exchange)
            }).FirstOrDefault();

            if (user == null)
                return View("Error"); //TODO: Message View

            var matches = user.RecieveFromMatches.Join(user.SendToMatches, x => x.ExchangeId, x => x.ExchangeId,
                (r, s) => new {
                    RecipientId = r.Id, RecipientName = r.Name, SenderId = s.Id, SenderName = s.Name,
                    ExchangeId = r.ExchangeId
                });

            var exchanges = user.Exchanges.GroupJoin(matches, x => x.Id, x => x.ExchangeId, (e, m) => new {
                Exchange = e, Match = m.FirstOrDefault()
            });

            ExchangeListVM vm = new ExchangeListVM {
                UserId = id,
                UserName = user.UserName,
                Exchanges = exchanges.OrderBy(x => x.Exchange.OpenDate).Select(x => new ExchangeVM { Id = x.Exchange.Id, Name = x.Exchange.Name, ReceiveFromId = x.Match?.RecipientId, ReceiveFromName = x.Match?.RecipientName, SendToId = x.Match?.SenderId, SendToName = x.Match?.SenderName}).ToList()
            };

            return View(vm);

        }

        private void createUserConfirmationEmail(string id) {

            EmailClient email = EmailClient.Create();

            var user = DAL.Context.Users.Where(x => x.Id == id).Select(x => new { Email = x.Email, Profile = x.Profile }).FirstOrDefault();

            if (user?.Profile == null)
                return;

            string phoneNumber = user.Profile.PhoneNumber?.PadLeft(10, ' ') ?? "          ";

            ProfileVM vm = new ProfileVM {
                Id = id,
                Name = user.Profile.FullName,
                Address = user.Profile.Address,
                City = user.Profile.City,
                State = user.Profile.State,
                Zip = user.Profile.Zip,
                DeliveryNotes = user.Profile.DeliveryNotes,

                RedditUsername = user.Profile.RedditUsername,
                UntappdUsername = user.Profile.UntappdUsername,

                References = user.Profile.References,
                //Wishlist = user.Profile.Wishlist,
                Comments = user.Profile.Comments,

                Piney = user.Profile.Piney,
                Juicy = user.Profile.Juicy,
                Tart = user.Profile.Tart,
                Funky = user.Profile.Funky,
                Malty = user.Profile.Malty,
                Roasty = user.Profile.Roasty,
                Sweet = user.Profile.Sweet,
                Smokey = user.Profile.Smokey,
                Spicy = user.Profile.Spicy,
                Crisp = user.Profile.Crisp,


                Phone = $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6, 4)}",
                Email = user.Email,

                UpdateDate = user.Profile.UpdateDate

            };

            string body = "<p>Thanks for updating your profile. Here's what we have on record for you:</p>" + RenderPartialToString(this, "__ViewProfile", vm);

            MailMessage message = new MailMessage {
                To = { new MailAddress(user.Email, user.Profile.FullName) },
                Bcc = { new MailAddress("redditbeeritforward@gmail.com"), new MailAddress("bdawgert@gmail.com") },
                From = new MailAddress("redditbeeritforward@gmail.com", "BeerItForward"),
                Subject = "BeerItForward Profile Complete",
                Body = body,
                IsBodyHtml = true
            };

            email.SMTP.Send(message);
        }
        

        public ActionResult MatchPreferences(string id, string userId) {

            int.TryParse(id, out int exchangeId);
            if (exchangeId == 0)
                exchangeId = BifSessionData.ExchangeId;

            userId = BifSessionData.IsInRole("ADMIN") ? userId ?? BifSessionData.Id : BifSessionData.Id;
            
            int exclusions = Convert.ToInt32(BifSessionData.Claims.FirstOrDefault(x => x.Type == "http://21brews.com/identity/claims/allowed-exclusions")?.Value ?? "2");
            if (userId != BifSessionData.Id)
                exclusions = Convert.ToInt32(DAL.Context.Claims.FirstOrDefault(x => x.Type == "http://21brews.com/identity/claims/allowed-exclusions")?.Value ?? "2");
            
            var allUsers = DAL.Context.Users.Where(x => x.UserStatus == IdentityUser.UserStatuses.Approved && x.Profile != null || x.Id == userId)
                .Select(x => new {
                    Id = x.Id,
                    Username = x.UserName ?? x.Profile.RedditUsername,
                    City = x.Profile.City,
                    State = x.Profile.State,
                    Zip = x.Profile.Zip,
                    x.Profile.ZipCode
                }).OrderBy(x => x.Username).ToList();

            var me = allUsers.FirstOrDefault(x => x.Id == userId);
            
            List<KeyValuePair<MatchPreferenceType, string>> preferences =  DAL.Context.MatchPreferences.Where(x => x.UserId == userId)
                .AsEnumerable().Select(x => new KeyValuePair<MatchPreferenceType, string>(x.PreferenceType, x.Value)).ToList();

                //Exchange exchange = DAL.Context.Exchanges.Find(exchangeId);

            MatchPreferencesVM vm = new MatchPreferencesVM {
                UserId = userId,
                //ExchangeName = exchange?.Name,
                AllUsers = allUsers.Select(x => new UserPublicProfile {
                    Id = x.Id,
                    Username = x.Username,
                    Location = $"{x.City}, {x.State}",
                    Zip = x.Zip,
                    Distance = DistanceTools.Haversine(x.ZipCode.Latitude, x.ZipCode.Longitude, me?.ZipCode.Latitude ?? 0m, me?.ZipCode.Longitude ?? 0m)

                }).ToList(),
                MatchPreferences = preferences,
                AllowedExclusions = exclusions
            };

            return View(vm);
        }

        public ActionResult Box(string id, string userId) {

            if (BifSessionData.UserStatus <= 0 || !BifSessionData.HasProfile) {
                ViewBag.Message = "Your user profile is still pending.  An administrator will review and approve your profile before you can use the Box Builder. Please check back soon.";
                return View("Message");
            }
            
            int.TryParse(id, out int exchangeId);
            if (exchangeId == 0)
                exchangeId = BifSessionData.ExchangeId;

            if (exchangeId == 0) {
                ViewBag.Message = "No Exchange selected.";
                return View("Message");
            }

            userId = BifSessionData.IsInRole("ADMIN") ? userId ?? BifSessionData.Id : BifSessionData.Id;

            Exchange exchange = DAL.Context.SignUps.Where(s => s.UserId == userId && s.ExchangeId == exchangeId).Select(x => x.Exchange).FirstOrDefault();

            if (exchange == null && exchangeId != 0)
                return RedirectToAction("", "Home");

            BoxVM vm = new BoxVM {
                UserId = userId,
                ExchangeId = exchangeId,
                ExchangeName = exchange?.Name ?? "Open",
                IsLocked = exchange?.ShipDate < DateTime.Now.AddDays(-14),
                Items = boxItems(userId, exchangeId),
            };

            return View(vm);
        }

        private List<BoxItem> boxItems(string userId, int exchangeId) {
            return DAL.Context.Items.Where(x => x.UserId == userId && x.ExchangeId == exchangeId)
                .Select(x => new BoxItem {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Format = x.Format,
                Cost = x.Cost,
                UntappdRating = x.UntappdRating == null ? (double?)null : Math.Round(x.UntappdRating ?? 0, 2),
                USOunces = x.USOunces
            }).ToList();
        }

        public JsonResult AddItem(BoxItem item, string userId, int exchangeId) {
            string id = BifSessionData.IsInRole("ADMIN") ? userId ?? BifSessionData.Id : BifSessionData.Id;

            Exchange exchange = DAL.Context.Exchanges.Find(BifSessionData.ExchangeId);
            if (exchange == null || exchange.CloseDate.AddDays(30) < DateTime.Today)
                return Json(new { Success = false, Message = "This Exchange is now closed. The box may no longer be edited"});

            if (item.UntappdId != null) {
                UntappdClient untappdClient = UntappdClient.Create();
                Beer beer = untappdClient.Lookup(item.UntappdId.Value);
                item.Name = $"{beer.Name} ({beer.Brewery.Name})";
                item.UntappdRating = beer.Rating;
            }

            Item entity = new Item {
                UserId = id,
                ExchangeId = exchangeId,
                Format = item.Format,
                Name = item.Name,
                Type = item.Type,
                UntappdId = item.UntappdId,
                USOunces = item.USOunces,
                Cost = item.Cost,
                UntappdRating = item.UntappdRating
            };

            DAL.Context.Items.Add(entity);
            DAL.Context.SaveChanges();

            return Json(new {
                Success = true,
                UserId = id,
                Id = entity.Id,
                ExchangeId = BifSessionData.ExchangeId,
                Format = item.Format,
                Name = item.Name,
                Type = item.Type,
                UntappdId = item.UntappdId,
                USOunces = item.USOunces,
                Cost = item.Cost,
                UntappdRating = item.UntappdRating == null ? null : Math.Round(item.UntappdRating ?? 0, 2).ToString("0.00")
            });

        }

        [HttpPost]
        public JsonResult DeleteItem(int id) {
            Item item = DAL.Context.Items.Find(id);
            if (item == null)
                return Json(new { Success = false });

            Exchange exchange = DAL.Context.Exchanges.Find(item.ExchangeId);
            if (exchange == null || exchange.CloseDate.AddDays(30) < DateTime.Today)
                return Json(new { Success = false, Message = "This Exchange is now closed. The box may no longer be edited" });

            DAL.Context.Items.Remove(item);
            DAL.Context.SaveChanges();

            return Json(new { Success = true });
        }

        [HttpGet]
        public PartialViewResult BoxSummary(string id, string userid) {
            int.TryParse(id, out int exchangeId);
            userid = BifSessionData.IsInRole("ADMIN") ? userid ?? BifSessionData.Id : BifSessionData.Id;

            Exchange exchange = DAL.Context.Exchanges.Find(exchangeId);

            List<Item> items = DAL.Context.Items.Where(x => x.UserId == userid && x.ExchangeId == exchangeId).ToList();

            BoxBuilder boxBuilder = new BoxBuilder(items)
                .SetMinimumOunces(exchange?.MinOunces)
                .SetMinimumBeerRating(exchange?.MinRating)
                .SetMinimumBoxRating(exchange?.MinBoxRating)
                .SetMinimumUnique(exchange?.MinUnique)
                .SetMinimumCost(exchange?.MinCost);
            
            return PartialView("__BoxSummary", boxBuilder);
        }

        [HttpPost]
        public JsonResult UpdateExclusion(string id, string userId) {

            userId = BifSessionData.IsInRole("ADMIN") ? userId ?? BifSessionData.Id : BifSessionData.Id;
            //int exclusions = Convert.ToInt32(BifSessionData.Claims.FirstOrDefault(x => x.Type == "http://21brews.com/identity/claims/allowed-exclusions")?.Value ?? "2");
            //if (userId != BifSessionData.Id)
            //    exclusions = Convert.ToInt32(DAL.Context.Claims.FirstOrDefault(x => x.Type == "http://21brews.com/identity/claims/allowed-exclusions")?.Value ?? "2");
            
            List<MatchPreference> preferences = DAL.Context.MatchPreferences.Where(x => x.UserId == userId && x.PreferenceType == MatchPreferenceType.NotUser).ToList();

            List<MatchPreference> existing = preferences.Where(x => x.Value == id).ToList();
            if (existing.Any()) {
                DAL.Context.MatchPreferences.RemoveRange(existing);
                DAL.Context.SaveChanges();
                return Json(new { Success = true, Action = "Unset", UserId = userId });
            }

            int allowedExclusions = Convert.ToInt32(BifSessionData.Claims.FirstOrDefault(x => x.Type == "http://21brews.com/identity/claims/allowed-exclusions")?.Value ?? "2");
            if (preferences.Count >= allowedExclusions)
                return Json(new { Success = false, Action = "", UserId = userId });

            DAL.Context.MatchPreferences.Add( new MatchPreference {
                UserId = userId,
                PreferenceType = MatchPreferenceType.NotUser,
                Value = id
            });
            DAL.Context.SaveChanges();

            return Json(new { Success = true, Action = "Set", UserId = userId });

        }

        //[HttpGet]
        //private JsonResult Search(string q) {
        //    UntappdClient untappdClient = UntappdClient.Create();
        //    var search = untappdClient.Search(q);

        //    var results = search.Beers.Items.Select(x => new {
        //        x.Beer.Id,
        //        Name = x.Beer.Name,
        //        x.Beer.Rating,
        //        Brewery = x.Brewery.Name
        //    });

        //    return Json(results, JsonRequestBehavior.AllowGet);

        //}

        private enum FlavorPrefernece
        {
            None = 0,
            LoveIt = 1,
            LikeIt = 2,
            Maybe = 3,
            No = 5
        }


        private bool signUp(int exchangeId, string userId, out string message)         {

            var exchange = DAL.Context.Exchanges.Where(x => x.Id == exchangeId)
                .Select(x => new { Exchange = x, SignUp = x.SignUps.Any(s => s.UserId == userId) }).FirstOrDefault();

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
    }
}