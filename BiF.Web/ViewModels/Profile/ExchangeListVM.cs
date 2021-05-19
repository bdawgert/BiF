using System.Collections.Generic;

namespace BiF.Web.ViewModels.Profile
{
    public class ExchangeListVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<ExchangeVM> Exchanges { get; set; }
    }

    public class ExchangeVM {

        public int Id { get; set; }
        public string Name { get; set; }
        public string SendToId { get; set; }
        public string SendToName { get; set; }
        public string ReceiveFromId { get; set; }
        public string ReceiveFromName { get; set; }

    }

}