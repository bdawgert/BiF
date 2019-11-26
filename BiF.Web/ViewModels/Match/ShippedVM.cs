using System;

namespace BiF.Web.ViewModels.Match
{
    public class ShippedVM
    {
        public string SenderId { get; set; }
        public string ExchangeId { get; set; }
        public string Carrier { get; set; }
        public string TrackingNo { get; set; }
        public DateTime ShipDate { get; set; }
    }
}