using System.Collections.Generic;

namespace BiF.Web.ViewModels.Exchanges
{
    public class AssignVM
    {

        public List<Assignment> Assignments { get; set; }

    }

    public class Assignment
    {
        public string SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string RecipientId { get; set; }
        public string RecipientUsername { get; set; }

    }

}