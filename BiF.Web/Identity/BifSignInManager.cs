using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Web;
using BiF.DAL.Extensions;
using BiF.DAL.Models;


namespace BiF.Web.Identity
{
    public class BifSignInManager : IDisposable
    {

        private static BifSignInManager _bifSignInManager;
        private BifUserManager _bifUserManager;

        private BifSignInManager(BifUserManager userManager) {
            _bifUserManager = userManager;
        }

        //public override Task<ClaimsIdentity> CreateUserIdentityAsync(IdentityUser user) {
        //    return user.GenerateUserIdentityAsync((BifUserManager)UserManager);
        //}

        public static BifSignInManager Create(BifUserManager userManager) {
            if (_bifSignInManager == null)
                _bifSignInManager = new BifSignInManager(userManager);
            return _bifSignInManager;
        }

        public SignInResult PasswordSignIn(string email, SecureString password, bool shouldLockout) {

            SignInResult signInResult = new SignInResult();

            IdentityUser user = _bifUserManager.FindByEmail(email);
            if (user == null || user.UserStatus < 0) {
                signInResult.Status = (SignInResult.SignInStatus) 0;
                return signInResult;
            }

            bool authenticated = user.PasswordHash == password.HashValue(user.Entropy);
            if (authenticated) {
                signInResult.Status |= SignInResult.SignInStatus.Authenticated;
            }

            return signInResult;
        }


        public SignInResult AutoSignIn(string email) {

            IdentityUser user = _bifUserManager.FindByEmail(email);

            SignInResult signInResult = new SignInResult();
            signInResult.Status |= SignInResult.SignInStatus.Authenticated;

            return signInResult;

        }

        public ClaimsPrincipal CreateUserPrincipal(IdentityUser user) {
            List<Claim> claims = user.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.Id));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            ClaimsIdentity userIdentity = new ClaimsIdentity(claims);
            
            HttpContext.Current.User = new ClaimsPrincipal(userIdentity);
            

            return new ClaimsPrincipal(userIdentity);
            
        }

        public void Dispose() {
            _bifSignInManager = null;
        }
    }
}