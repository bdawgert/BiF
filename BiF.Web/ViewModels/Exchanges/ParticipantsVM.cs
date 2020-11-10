using System.Collections.Generic;
using BiF.Web.ViewModels.Home;

namespace BiF.Web.ViewModels.Exchanges
{
    public class ParticipantsVM
    {
        public string ExchangeName { get; set; }
        public List<ParticipantVM> Participants { get; set; }

    }
}