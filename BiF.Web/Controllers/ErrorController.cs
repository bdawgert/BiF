using System;
using System.Net.Mail;
using System.Web.Mvc;
using BiF.Web.Utilities;

namespace BiF.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index() {
            Exception lastError = Server.GetLastError()?.GetBaseException();

            bool emailSent = false;
            if (lastError != null) {
                try {
                    EmailClient email = EmailClient.Create();

                    MailMessage message = new MailMessage {
                        To = { new MailAddress("bdawgert@gmail.com") },
                        From = new MailAddress("redditbeeritforward@gmail.com", "BeerItForward"),
                        Subject = $"BiF Error: {lastError.Message}",
                        Body = $"<p>{lastError.Message}</p><p>{lastError.StackTrace}</p>",
                        IsBodyHtml = true
                    };

                    email.SMTP.Send(message);
                    emailSent = true;
                }
                catch {
                    // ignored
                }
            }

            string baseMessage =
                "<p>Well, this is embarassing.  We weren't expecting an unexpected error.  Not now.  Not like this.</p>";
            string notfiyMessage = emailSent
                ? "<p>Don't worry, we've called the police, and ambulance, and the National Guard.  Everything will be OK.</p>" : "";
            string stackMessage = $"<!-- {lastError?.Message}\n {lastError?.StackTrace} -->";

            ViewBag.MessageTitle = "Ummmm.... Error?";
            ViewBag.Message = baseMessage + notfiyMessage + stackMessage;


            return View("Message");
        }

        public ActionResult NotFound() {
            ViewBag.MessageTitle = "Hello?";
            ViewBag.Message = "<p>Hello {friend},</p>" + 
            "<p>You seem to be somewhere that isn't anywhere. You're lost. Or we gave you bad directions.</p>" + 
            "<p>Either way, click on that HOME button up top and get back to a safe place.</p>" + 
            "<p>{fakedFriendlyValediction},</p>" + 
            "<p>The Mods</p>";
            
            return View("Message");
        } 

    }
}