
using System;

namespace BiF.Web.ViewModels.Exchanges
{
    public class SignUpVM
    {
        public int ExchangeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime OpenDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? MatchDate { get; set; }
        public DateTime CloseDate { get; set; }

        public double? MinOunces { get; set; }
        public double? MinCost { get; set; }
        public double? MinRating { get; set; }
        public double? MinBoxRating { get; set; }
        public int? MinUnique { get; set; }

        public string Comment { get; set; }

        public DateTime? SignUpDate { get; set; }

        public bool IsAcknowledged { get; set; }



    }
}