using System;
using System.Collections.Generic;

namespace BiF.Web.ViewModels.Exchanges
{
    public class ViewStatusVM
    {
        public List<ShipmentStatus> ShipmentStatuses { get; set; }
    }


    public class ShipmentStatus
    {
        public string SenderId { get; set; }
        public string Sender { get; set; }
        public string Carrier { get; set; }
        public string TrackingNo { get; set; }
        public DateTime? ShipDate { get; set; }
        public string Recipient { get; set; }
    }

}