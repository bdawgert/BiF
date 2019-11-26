using System.Collections.Generic;
using System.Web.Mvc;

namespace BiF.Web.ViewModels.Exchanges
{
    public class AssignListVM
    {
        public string SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string MatchId { get; set; }
        public List<SelectListItem> AvailableUsers { get; set; }
    }
}