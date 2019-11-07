using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace BiF.Web.ViewModels.Manage
{
    public class ManageLoginsVM
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        //public IList<AuthenticationDescription> OtherLogins { get; set; }
    }
}