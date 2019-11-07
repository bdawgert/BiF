using System;
using System.Collections.Generic;
using System.Security;
using BiF.DAL.Extensions;
using BiF.DAL.Models;

namespace BiF.Web.Identity
{
    public class BifUserManager : IDisposable //: UserManager<BifIdentityUser>
    {
        private static BifUserManager _bifUserManager;
        private BifUserStore _bifUserStore;

        private BifUserManager() {
            _bifUserStore = BifUserStore.Create();
        }

        public IdentityUser User => _bifUserStore.User;

        public UserManageResult CreateUser(string email, SecureString password) {

            IdentityUser user = _bifUserStore.LoadUserByEmail(email);
            if (user != null)
                return new UserManageResult {
                    Success = false,
                    Errors = new List<string> { "User already exists." }
                };

            bool passwordIsValid = validatePassword(password.Unsceure());
            if (!passwordIsValid)
                return new UserManageResult {
                    Success = false,
                    Errors = new List<string> {"Password does not meet minimum complexity requirements."}
                };

            byte[] salt = CryptoTools.CreateSalt();
            user = new IdentityUser {
                Id = Guid.NewGuid().ToString(),
                Entropy = salt,
                Email = email.Trim(),
                PasswordHash = password.HashValue(salt)
            };
            _bifUserStore.Add(user);
            _bifUserStore.Update();
            
            return new UserManageResult {
                Success = true
            };

        }

        public IdentityUser FindById(string id) {
            return _bifUserStore.LoadUserById(id);
        }

        public IdentityUser FindByEmail(string email) {
            return _bifUserStore.LoadUserByEmail(email);
        }

        public UserManageResult AddClaim(string type, string value) {
            IdentityUser user = _bifUserStore.User;
            if (user == null) 
                return  new UserManageResult {
                    Success = false,
                    Errors = new List<string> { "" }
                };


            user.Claims.Add(new IdentityClaim {
                Type = type,
                Value = value
            });

            _bifUserStore.Update();

            return new UserManageResult {
                Success = true
            };
        }

        public UserManageResult AddRole(string name) {
            IdentityUser user = _bifUserStore.User;
            if (user == null)
                return new UserManageResult {
                    Success = false,
                    Errors = new List<string> { "" }
                };


            user.Roles.Add(new IdentityRole {
                Name = name
            });

            _bifUserStore.Update();

            return new UserManageResult {
                Success = true
            };
        }

        private bool validatePassword(string password) {

            if (password.Length >= 8)
                return true;
            return false;
        }

        public static BifUserManager Create() {

            if (_bifUserManager != null)
                return _bifUserManager;

            _bifUserManager = new BifUserManager {
                //UserLockoutEnabledByDefault = true,
                //DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5),
                //MaxFailedAccessAttemptsBeforeLockout = 5
            };

            return _bifUserManager;
        }

        

        //public class EmailService : IIdentityMessageService
        //{
        //    public Task SendAsync(IdentityMessage message)
        //    {
        //        // Plug in your email service here to send an email.
        //        return Task.FromResult(0);
        //    }
        //}

        //public class SmsService : IIdentityMessageService
        //{
        //    public Task SendAsync(IdentityMessage message)
        //    {
        //        // Plug in your SMS service here to send a text message.
        //        return Task.FromResult(0);
        //    }
        //}

        public void Dispose() {
            _bifUserManager = null;
        }
    }
}