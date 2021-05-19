using System.Collections.Generic;
using System.Security.AccessControl;

namespace BiF.Web.ViewModels.Exchanges
{
    public class AssignVM
    {
        public int ExchangeId { get; set; }
        public List<Assignment> Assignments { get; set; }

    }

    public class Assignment
    {
        public string SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderLocation { get; set; }
        public string RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string Carrier { get; set; }
        public string TrackingNo { get; set; }

    }

}