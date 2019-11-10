using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiF.Web.ViewModels.Administration
{
    public class IndexVM
    {
        public List<UserInformation> Users { get; set; }

    }

    public class UserInformation
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string[] Roles { get; set; }
        public bool Approved => UserStatus > 0;
        public int UserStatus { get; set; }
        public bool HasProfile { get; set; }

    }

}