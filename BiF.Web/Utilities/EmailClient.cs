using System.Net.Mail;

namespace BiF.Web.Utilities
{
    public class EmailClient
    {
        private static EmailClient _client;

        public SmtpClient SMTP { get; private set; }

        private EmailClient() {

            string smtpPassword = KeyVault.GetSecret("sendgrid-apikey").Result;

            SMTP = new SmtpClient {
                Host = "smtp.sendgrid.net",
                Port = 587,
                //EnableSsl = true,
                Credentials = new System.Net.NetworkCredential("apikey", smtpPassword)
            };

        }

        public static EmailClient Create() {
            if (_client != null)
                return _client;

            return _client = new EmailClient();

        }

    }
}