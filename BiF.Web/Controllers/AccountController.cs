using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BiF.DAL.Models;
using BiF.DAL.Extensions;
using BiF.Web.Identity;
using BiF.Web.Utilities;
using BiF.Web.ViewModels.Account;

namespace BiF.Web.Controllers
{
    public class AccountController : BaseController
    {
        private BifSignInManager _signInManager;
        private BifUserManager _userManager;

        public AccountController() { }

        public AccountController(BifUserManager userManager, BifSignInManager signInManager )
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
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model, string returnUrl) {
            if (!ModelState.IsValid)
                return View(model);

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            SignInResult signInResult = SignInManager.PasswordSignIn(model.Email, model.Password.Secure(), true);

            if (signInResult.Status.HasFlag(SignInResult.SignInStatus.Authenticated)) {
                ClaimsPrincipal userPrincipal = SignInManager.CreateUserPrincipal(UserManager.User);
                FormsAuthentication.SetAuthCookie(userPrincipal.Identity.Name, model.RememberMe);
                HttpContext.User = userPrincipal;

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, userPrincipal.Identity.Name, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(20), model.RememberMe, string.Join(";", UserManager.User.Roles.Select(x => x.Name)) );
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(authCookie);
                
                return redirectToLocal(returnUrl);
            }

            if (signInResult.Status.HasFlag(SignInResult.SignInStatus.LockedOut)) 
                return View("Lockout");

            ModelState.AddModelError("", "Invalid Username or Password");
            return View(model);
            
        }
        
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register() {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model) {
            if (ModelState.IsValid) {

                UserManageResult userManagerResult = UserManager.CreateUser(model.Email, model.Password.Secure());

                if (userManagerResult.Success) {
                    UserManager.AddClaim("http://21brews.com/identity/claims/account-state", "registered");
                    
                    ClaimsPrincipal userPrincipal = SignInManager.CreateUserPrincipal(UserManager.User);
                    FormsAuthentication.SetAuthCookie(userPrincipal.Identity.Name, false);
                    HttpContext.User = userPrincipal;
                    
                    if (!DAL.Context.Profiles.Any(x => x.Id == userPrincipal.Identity.Name))
                        return RedirectToAction("Index", "Profile");

                    return RedirectToAction("Index", "Home");
                }
                addErrors(userManagerResult);
            }

            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    IdentityResult result = UserManager.ConfirmEmail(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid) {
                IdentityUser user = UserManager.FindByEmail(model.Email);
                if (user == null ) 
                    return View("ForgotPasswordConfirmation");
                
                EmailClient email = EmailClient.Create();

                string token = HttpUtility.HtmlEncode(user.PasswordHash.Substring(0, 32));
                string resetUrl = $@"https://beeritforward.azurewebsites.net{Url.Action("ResetPassword", "Account", new { token = token })}";
                string messageBody = "<p>A Password Reset has been requested for your account.  Click the following link to complete the reset:</p>" +
                                     $"<a href=\"{resetUrl}\">{resetUrl}</a>";

                MailMessage message = new MailMessage {
                    To = { new MailAddress(user.Email, user.Profile.FullName) },
                    Bcc = { new MailAddress("redditbeeritforward@gmail.com"), new MailAddress("bdawgert@gmail.com") },
                    From = new MailAddress("redditbeeritforward@gmail.com", "BeerItForward"),
                    Subject = "BeerItForward Password Reset",
                    Body = messageBody,
                    IsBodyHtml = true
                };

                email.SMTP.Send(message);

                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation() {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string token)
        {

            if (token == null || token.Length < 32)
                return View("Unauthorized");

            IdentityUser user = DAL.Context.Users.FirstOrDefault(x => x.PasswordHash.StartsWith(token));

            if (user == null)
                return View("Unauthorized");

            ResetPasswordVM vm = new ResetPasswordVM {
                Code = token
            };

            return View(vm);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordVM vm) {
            if (!ModelState.IsValid) {
                return View(vm);
            }

            IdentityUser user = DAL.Context.Users.FirstOrDefault(x => x.Email == vm.Email && x.PasswordHash.StartsWith(vm.Code) && x.UserStatus > 0);
            if (user == null) 
                return View("Unauthorized");

            UserManageResult result = UserManager.SetPassword(user.Id, vm.Password.Secure());

            if (result.Success) 
                return RedirectToAction("ResetPasswordConfirmation", "Account");

            addErrors(result);
            return View();
        }


        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation() {
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff() {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region Helpers

        private void addErrors(IBifResult result) {
            if (result.Errors == null)
                return;
            foreach (string error in result.Errors)
                ModelState.AddModelError("", error);
        }

        private ActionResult redirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
    
}