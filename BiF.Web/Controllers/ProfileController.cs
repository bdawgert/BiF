using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BiF.DAL.Models;
using BiF.Web.ViewModels;

namespace BiF.Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        [HttpGet]
        public ActionResult Index() {

            string id = BifSessionData.Id;
            var user = DAL.Context.Users.Where(x => x.Id == id).Select(x => new { Email = x.Email, Profile = x.Profile}).FirstOrDefault();

            if (user == null)
                return View("Error"); //TODO: Message View

            if (user.Profile == null) 
                return View(new ProfileVM { Email = user.Email });

            ProfileVM vm = new ProfileVM {
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

                Phone = user.Profile?.PhoneNumber,
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

            string id = User.Identity.Name;

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

            profile.Comments = vm.Comments;
            //profile.Wishlist = vm.Wishlist;
            profile.UpdateDate = DateTime.UtcNow;

            if (profile.CreateDate == null) {
                profile.CreateDate = DateTime.UtcNow;
                DAL.Context.Profiles.Add(profile);
            }

            DAL.Context.SaveChanges();

            return RedirectToAction("Index", "Manage");
        }


    }
}