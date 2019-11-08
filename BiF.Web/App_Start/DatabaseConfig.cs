using System.Configuration;
using System.Threading.Tasks;
using BiF.Web.Identity;
using System.Web;
using BiF.Web.Utilities;

namespace BiF.Web
{
    public static class DatabaseConfig 
    {

        public static void SetConnectionString() {
            string connectionString = ConfigurationManager.ConnectionStrings["BifDbContext"].ConnectionString;

            if (!connectionString.Contains("{0}")) {
                HttpContext.Current.Application.Add("connectionString", connectionString);
                return;
            }

            string username = KeyVault.GetSecret("db-username").Result;
            string password = KeyVault.GetSecret(username).Result;
            HttpContext.Current.Application.Add("connectionString", string.Format(connectionString, username, password));
        }

    }
}