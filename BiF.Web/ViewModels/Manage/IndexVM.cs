using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace BiF.Web.ViewModels.Manage
{
    public class IndexVM
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public bool BrowserRemembered { get; set; }
    }
}