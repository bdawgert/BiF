using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BiF.DAL.Models;
using BiF.Web.Utilities;
using BiF.Web.ViewModels;

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

            var user = DAL.Context.Users.Where(x => x.Id == id).Select(x => new { Email = x.Email, Profile = x.Profile}).FirstOrDefault();

            if (user == null)
                return View("Error"); //TODO: Message View

            if (user.Profile == null) 
                return View(new ProfileVM { Email = user.Email });

            string phoneNumber = user.Profile?.PhoneNumber?.PadLeft(10, ' ') ?? "          ";

            ProfileVM vm = new ProfileVM {
                Id = id,
                Name = user.Profile.FullName,
                Address = user.Profile?.Address,
                City = user.Profile?.City,
                State = user.Profile?.State,
                Zip = user.Profile?.Zip,

                RedditUsername = user.Profile?.RedditUsername,
                UntappdUsername = user.Profile?.UntappdUsername,

                References = user.Profile?.References,
                //Wishlist = user.Profile?.Wishlist,
                Comments = user.Profile?.Comments,

                Piney = user.Profile?.Piney,
                Juicy = user.Profile?.Juicy,
                Tart = user.Profile?.Tart,
                Funky = user.Profile?.Funky,
                Malty = user.Profile?.Malty,
                Roasty = user.Profile?.Roasty,
                Sweet = user.Profile?.Sweet,
                Smokey = user.Profile?.Smokey,
                Spicy = user.Profile?.Spicy,
                Crisp = user.Profile?.Crisp,

                
                Phone = $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6,4)}",
                Email = user.Email,

                UpdateDate = user.Profile?.UpdateDate

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
                DAL.Context.Profiles.Add(profile);
            }

            DAL.Context.SaveChanges();

            if (id == BifSessionData.Id)// Only send if edited by self
                createConfirmationEmail(id);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ViewProfile(string id) {
            var user = DAL.Context.Users.Where(x => x.Id == id).Select(x => new { Email = x.Email, Profile = x.Profile }).FirstOrDefault();

            string phoneNumber = user.Profile?.PhoneNumber?.PadLeft(10, ' ') ?? "          ";

            ProfileVM vm = new ProfileVM
            {
                Id = id,
                Name = user.Profile.FullName,
                Address = user.Profile?.Address,
                City = user.Profile?.City,
                State = user.Profile?.State,
                Zip = user.Profile?.Zip,

                RedditUsername = user.Profile?.RedditUsername,
                UntappdUsername = user.Profile?.UntappdUsername,

                References = user.Profile?.References,
                //Wishlist = user.Profile?.Wishlist,
                Comments = user.Profile?.Comments,

                Piney = user.Profile?.Piney,
                Juicy = user.Profile?.Juicy,
                Tart = user.Profile?.Tart,
                Funky = user.Profile?.Funky,
                Malty = user.Profile?.Malty,
                Roasty = user.Profile?.Roasty,
                Sweet = user.Profile?.Sweet,
                Smokey = user.Profile?.Smokey,
                Spicy = user.Profile?.Spicy,
                Crisp = user.Profile?.Crisp,

                Phone = $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6, 4)}",
                Email = user.Email,

                UpdateDate = user.Profile?.UpdateDate

            };

            return View("View", vm);
        }

        private void createConfirmationEmail(string id) {

            EmailClient email = EmailClient.Create();

            var user = DAL.Context.Users.Where(x => x.Id == id).Select(x => new { Email = x.Email, Profile = x.Profile }).FirstOrDefault();

            string phoneNumber = user.Profile?.PhoneNumber?.PadLeft(10, ' ') ?? "          ";

            ProfileVM vm = new ProfileVM {
                Id = id,
                Name = user.Profile.FullName,
                Address = user.Profile?.Address,
                City = user.Profile?.City,
                State = user.Profile?.State,
                Zip = user.Profile?.Zip,

                RedditUsername = user.Profile?.RedditUsername,
                UntappdUsername = user.Profile?.UntappdUsername,

                References = user.Profile?.References,
                //Wishlist = user.Profile?.Wishlist,
                Comments = user.Profile?.Comments,

                Piney = user.Profile?.Piney,
                Juicy = user.Profile?.Juicy,
                Tart = user.Profile?.Tart,
                Funky = user.Profile?.Funky,
                Malty = user.Profile?.Malty,
                Roasty = user.Profile?.Roasty,
                Sweet = user.Profile?.Sweet,
                Smokey = user.Profile?.Smokey,
                Spicy = user.Profile?.Spicy,
                Crisp = user.Profile?.Crisp,


                Phone = $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6, 4)}",
                Email = user.Email,

                UpdateDate = user.Profile?.UpdateDate

            };

            string body = "<p>Thanks for updating your profile. Here's what we have on record for you:</p>" + RenderPartialToString(this, "__ViewProfile", vm);

            MailMessage message = new MailMessage {
                To = { new MailAddress(user.Email, user.Profile.FullName) },
                From = new MailAddress("redditbeeritforward@gmail.com", "BeerItForward"),
                Subject = "BeerItForward Profile Complete",
                Body = body,
                IsBodyHtml = true
            };

            email.SMTP.Send(message);


        }

    }
}