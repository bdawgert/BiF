using System.Collections.Generic;
using System.Web.Mvc;

namespace BiF.Web.ViewModels.Manage
{
    public class ConfigureTwoFactorVM
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
    }
}