using System;

namespace BiF.Web.ViewModels.Match
{
    public class ShippingNoticeVM
    {

        public string SenderUsername { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientEmail { get; set; }

        public string ExchangeName { get; set; }
        public string Carrier { get; set; }
        public string TrackingNo { get; set; }
    }
}