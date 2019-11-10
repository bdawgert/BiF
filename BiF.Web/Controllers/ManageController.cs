using System.Web.Mvc;
using BiF.DAL.Extensions;
using BiF.DAL.Models;
using BiF.Web.Identity;
using BiF.Web.ViewModels.Manage;

namespace BiF.Web.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private BifSignInManager _signInManager;
        private BifUserManager _userManager;

        public ManageController() {
        }

        public ManageController(BifUserManager userManager, BifSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public BifSignInManager SignInManager {
            get => _signInManager ?? BifSignInManager.Create(UserManager);
            private set => _signInManager = value;
        }

        public BifUserManager UserManager {
            get => _userManager ?? BifUserManager.Create();
            private set => _userManager = value;
        }

        //
        // GET: /Manage/Index
        public ActionResult Index()
        {

            IndexVM model = new IndexVM {
                HasPassword = hasPassword(),
            };
            return View(model);
        }

        public ActionResult ChangePassword() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM vm) {
            if (!ModelState.IsValid)
                return View(vm);

            BifUserManager userManager = BifUserManager.Create();
            UserManageResult userManageResult = userManager.SetPassword(BifSessionData.Id, vm.NewPassword.Secure());

            addErrors(userManageResult);

            if (!ModelState.IsValid)
                return View(vm);

            return RedirectToAction("", "Home");
        }

        public ActionResult ChangeEmail() {
            ChangeEmailVM vm = new ChangeEmailVM {
                Email = BifSessionData.Email
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmail(ChangeEmailVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            BifUserManager userManager = BifUserManager.Create();
            IdentityUser user = userManager.FindById(BifSessionData.Id);

            user.Email = vm.Email;

            DAL.Context.SaveChanges();

            return RedirectToAction("", "Profile");
        }


        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        
        private void addErrors(IBifResult result) {
            foreach (string error in result.Errors) {
                ModelState.AddModelError("", error);
            }
        }

        private bool hasPassword() {
            IdentityUser user = UserManager.FindById(User.Identity.Name);
            if (user != null)
                return user.PasswordHash != null;
            return false;
        }

#endregion
    }
}