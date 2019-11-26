using System;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

            var match = DAL.Context.Matches.Where(x => x.SenderId == id && x.ExchangeId == 2)
                .Select(x => new { Profile = x.Recipient.Profile, Email = x.Recipient.Email, Match = x } ).FirstOrDefault();

            if (match == null) {
                ViewBag.MessageTitle = "No Matches Yet";
                ViewBag.Message = "Matches should be ready on December 4. Don't worry, we'll let you know when they're posted. ";
                return View("Message");
            }

            string phoneNumber = match.Profile.PhoneNumber?.PadLeft(10, ' ');

            MatchVM vm = new MatchVM {
                Name = match.Profile.FullName,
                Address = match.Profile.Address,
                City = match.Profile.City,
                State = match.Profile.State,
                Zip = match.Profile.Zip,

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

                Phone = phoneNumber == null ? null : $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6, 4)}",
                Email = match.Email,

                SenderId = id,
                Carrier = match.Match.Carrier,
                TrackingNo = match.Match.TrackingNo,
                ShipDate = match.Match.ShipDate,

            };

            return View("Profile", vm);
        }

        [HttpPost]
        public JsonResult MarkShipped(ShippedVM vm) {

            string id = BifSessionData.IsInRole("ADMIN") ? vm.SenderId ?? BifSessionData.Id : BifSessionData.Id;

            var match = DAL.Context.Matches.Where(x => x.ExchangeId == 2 && x.SenderId == id)
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