using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace BiF.Web.Utilities
{
    public class EmailClient
    {
        private static EmailClient _client;

        public SmtpClient SMTP { get; private set; }

        private EmailClient() {

            string smtpPassword = KeyVault.GetSecret("redditbeeritforward-gmail-com").Result;

            SMTP = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential("redditbeeritforward@gmail.com", smtpPassword)
            };

        }

        public static EmailClient Create() {
            if (_client != null)
                return _client;

            return _client = new EmailClient();

        }

    }
}