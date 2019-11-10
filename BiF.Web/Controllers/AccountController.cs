using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
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
        public async Task<ActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = UserManager.FindByEmail(model.Email);
                if (user == null )
                    //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");

                string smtpPassword = await KeyVault.GetSecret("redditbeeritforward-gmail-com");

            SmtpClient smtp = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new System.Net.NetworkCredential("redditbeeritforward@gmail.com", smtpPassword)

            };
                MailMessage message = new MailMessage("BeerItForward <redditbeeritforward@gmail.com>", null);


            smtp.Send(message);

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordVM model)
        //{
        //    if (!ModelState.IsValid) {
        //        return View(model);
        //    }
        //    var user = UserManager.FindByEmail(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = UserManager.ResetPassword(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    addErrors(result);
        //    return View();
        //}


        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
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