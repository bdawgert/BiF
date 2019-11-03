using System.Linq;
using System.Web.Mvc;
using BiF.Web.ViewModels;
using Microsoft.AspNet.Identity;

namespace BiF.Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        public ActionResult Index() {

            string userId = User.Identity.GetUserId();

            var user = DAL.Context.Users.Where(x => x.Id == userId).Select(x => new { Phone = x.PhoneNumber, Email = x.Email, Profile = x.Profile}).FirstOrDefault();
            if (user == null)
                return View("Error"); //TODO: Message View

            ProfileVM vm = new ProfileVM {
                Address = user.Profile.Address,
                City = user.Profile.City,
                State = user.Profile.State,
                Zip = user.Profile.Zip,

                RedditUsername = user.Profile.RedditUsername,
                UntappdUsername = user.Profile.UntappdUsername,

                References = user.Profile.References,
                Wishlist = user.Profile.Wishlist,
                Comments = user.Profile.Comments,

                Piney = user.Profile.Piney,
                Juicey = user.Profile.Juicey,
                Tart = user.Profile.Tart,
                Funky = user.Profile.Funky,
                Malty = user.Profile.Malty,
                Roasty = user.Profile.Roasty,
                Sweet = user.Profile.Sweet,
                Smokey = user.Profile.Smokey,
                Spicy = user.Profile.Spicy,
                Crisp = user.Profile.Crisp,

                Phone = user.Phone,
                Email = user.Email

            };

            return View(vm);
        }
    }
}