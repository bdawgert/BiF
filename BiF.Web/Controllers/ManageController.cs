using System.Web.Mvc;
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

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> ChangePassword(ChangePasswordVM model)
    //    {
    //        if (!ModelState.IsValid)
    //            return View(model);

    //        UserManageResult userManageResult = UserManager.ChangePassword(model.OldPassword, model.NewPassword);
    //        if (userManageResult.Success)
    //            return RedirectToAction("Index", new { Message = "Password successfully updated."});

    //        addErrors(userManageResult);
    //        return View(model);
    //}

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
        
        //private void addErrors(IBifResult result) {
        //    foreach (string error in result.Errors) {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        private bool hasPassword() {
            IdentityUser user = UserManager.FindById(User.Identity.Name);
            if (user != null)
                return user.PasswordHash != null;
            return false;
        }

#endregion
    }
}