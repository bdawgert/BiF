using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using BiF.DAL.Models;
using BiF.Web.Utilities;
using BiF.Web.ViewModels.Match;

namespace BiF.Web.Controllers
{
    public class MatchController : BaseController
    {
        [Authorize]
        public ActionResult Index(string id) {

            id = BifSessionData.IsInRole("ADMIN") ? id ?? BifSessionData.Id : BifSessionData.Id;

            var match = DAL.Context.Exchanges.Where(x => x.Id == BifSessionData.ExchangeId)
                .Select(x => new {
                    Exchange = x,
                    Match = x.Matches.FirstOrDefault(m => m.SenderId == id),
                }).Select(x => new {
                    Exchange = x.Exchange,
                    Match = x.Match,
                    Profile = x.Match.Recipient.Profile,
                    SignUpComment = x.Match.Recipient.Profile.SignUps.FirstOrDefault().Comment,
                    Email = x.Match.Recipient.Email
                }).FirstOrDefault();

            //var match = DAL.Context.Matches.Where(x => x.SenderId == id && x.ExchangeId == BifSessionData.ExchangeId)
            //    .Select(x => new {
            //        Profile = x.Recipient.Profile, 
            //        Email = x.Recipient.Email, 
            //        Match = x,
            //        Exchange = x.Exchange
            //    } ).FirstOrDefault();

            if (match?.Exchange.MatchDate != null && match.Exchange.MatchDate > DateTime.UtcNow.AddHours(-14)) { // UTC -14 on a date is 9:00AM EST
                ViewBag.MessageTitle = "No Matches Yet";
                ViewBag.Message = $"Matches should be ready on {match.Exchange.MatchDate?.ToLongDateString()}. Don't worry, we'll let you know when they're posted. ";
                return View("Message");
            }

            if (match.Match == null) {
                ViewBag.MessageTitle = "No Matches Yet";
                ViewBag.Message = $"Matches should be ready around {match.Exchange?.MatchDate?.ToLongDateString()}. Don't worry, we'll let you know when they're posted. ";
                return View("Message");
            }

            string phoneNumber = match.Profile.PhoneNumber?.PadLeft(10, ' ');

            DateTime hideDate = match.Exchange.ShipDate?.AddDays(14) ?? match.Exchange.MatchDate?.AddDays(28) ?? match.Exchange.CreateDate.AddDays(60);

            MatchVM vm = new MatchVM {
                Name = match.Profile.FullName,
                Address = hideDate >= DateTime.Now ? match.Profile.Address : "",
                City = hideDate >= DateTime.Now ? match.Profile.City : "",
                State = hideDate >= DateTime.Now ? match.Profile.State : "",
                Zip = hideDate >= DateTime.Now ? match.Profile.Zip : "",

                RedditUsername = match.Profile.RedditUsername,
                UntappdUsername = match.Profile.UntappdUsername,

                References = match.Profile.References,
                //Wishlist = match.Profile.Wishlist,
                Comments = match.Profile.Comments,

                Piney = match.Profile.Piney,
                Juicy = match.Profile.Juicy,
                Tart = match.Profile.Tart,
                Funky = match.Profile.Funky,
                Malty = match.Profile.Malty,
                Roasty = match.Profile.Roasty,
                Sweet = match.Profile.Sweet,
                Smokey = match.Profile.Smokey,
                Spicy = match.Profile.Spicy,
                Crisp = match.Profile.Crisp,

                Phone = hideDate >= DateTime.Now ? phoneNumber == null ? null : $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6, 4)}" : "",
                Email = hideDate >= DateTime.Now ? match.Email : "",

                SenderId = id,
                Carrier = match.Match.Carrier,
                TrackingNo = match.Match.TrackingNo,

                ExchangeName = match.Exchange.Name,
                ShipDate = match.Match.ShipDate,
                CloseDate = match.Exchange.CloseDate

            };

            return View("Index", vm);
        }

        [HttpPost]
        public JsonResult MarkShipped(ShippedVM vm) {

            string id = BifSessionData.IsInRole("ADMIN") ? vm.SenderId ?? BifSessionData.Id : BifSessionData.Id;

            var match = DAL.Context.Matches.Where(x => x.ExchangeId == BifSessionData.ExchangeId && x.SenderId == id)
                .Select(x => new { Match = x, ExchangeName = x.Exchange.Name, SenderEmail = x.Sender.Email, SenderUsername = x.Sender.Profile.RedditUsername, RecipientEmail = x.Recipient.Email, RecipientUsername = x.Recipient.Profile.RedditUsername })
                .FirstOrDefault();

            if (match == null)
                return Json(new {Success = false});

            match.Match.Carrier = vm.Carrier;
            match.Match.TrackingNo = vm.TrackingNo;
            match.Match.ShipDate = DateTime.Now;

            DAL.Context.SaveChanges();
            
            ShippingNoticeVM noticeVM = new ShippingNoticeVM {
                Carrier = match.Match.Carrier,
                TrackingNo = match.Match.TrackingNo,
                ExchangeName = match.ExchangeName,
                SenderEmail = match.SenderEmail,
                SenderUsername = match.SenderUsername,
                RecipientEmail = match.RecipientEmail,
                RecipientUsername = match.RecipientUsername
            };

            sendShippingEmails(noticeVM);

            return Json(new { Success = true });

        }
        
        private void sendShippingEmails(ShippingNoticeVM vm) {

            EmailClient email = EmailClient.Create();

            string confirmationBody = RenderPartialToString(this, "__ShippingConfirmation", vm);
            string notificationBody = RenderPartialToString(this, "__ShippingNotification", vm);

            MailMessage confirmationMessage = new MailMessage
            {
                To = { new MailAddress(vm.SenderEmail, vm.SenderUsername) },
                CC = { new MailAddress("redditbeeritforward@gmail.com") },
                Bcc = { new MailAddress("bdawgert@gmail.com") },
                From = new MailAddress("redditbeeritforward@gmail.com", "BeerItForward"),
                Subject = $"{vm.ExchangeName} Shipping Confirmation",
                Body = confirmationBody,
                IsBodyHtml = true
            };
            
            MailMessage notificationMessage = new MailMessage
            {
                To = { new MailAddress(vm.RecipientEmail, vm.RecipientUsername) },
                CC = { new MailAddress("redditbeeritforward@gmail.com") },
                Bcc = { new MailAddress("bdawgert@gmail.com") },
                From = new MailAddress("redditbeeritforward@gmail.com", "BeerItForward"),
                Subject = $"{vm.ExchangeName} Shipping Notification",
                Body = notificationBody,
                IsBodyHtml = true
            };
#if (DEBUG)
#else
            email.SMTP.Send(confirmationMessage);
            email.SMTP.Send(notificationMessage);
#endif

        }

    }
}