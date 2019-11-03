using System;
using BiF.DAL.Concrete;
using BiF.DAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace BiF.Web.Owin
{
    public class BifUserManager : UserManager<BifIdentityUser>
    {
        public BifUserManager(IUserStore<BifIdentityUser> store) : base(store) { }



        public static BifUserManager Create(IdentityFactoryOptions<BifUserManager> options, IOwinContext context)
        {
            var manager = new BifUserManager(new UserStore<BifIdentityUser>(context.Get<BifDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<BifIdentityUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<BifIdentityUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<BifIdentityUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<BifIdentityUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
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

    }
}