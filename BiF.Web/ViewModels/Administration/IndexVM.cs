using System;
using System.Collections.Generic;

namespace BiF.Web.ViewModels.Administration
{
    public class IndexVM
    {
        public List<ExchangeInformation> Exchanges { get; set; }
        public List<UserInformation> Users { get; set; }
    }

    public class UserInformation
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string[] Roles { get; set; }
        public bool Approved => UserStatus > 0;
        public int Rating { get; set; }
        public int UserStatus { get; set; }
        public bool HasProfile { get; set; }
        public string AllowedExclusions { get; set; }

    }

    public class ExchangeInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
    }

}