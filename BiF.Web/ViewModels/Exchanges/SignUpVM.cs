
using System;
using System.ComponentModel.DataAnnotations;

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

        public double MinOunces { get; set; }
        public double MinCost { get; set; }
        public double MinRating { get; set; }
        
        public DateTime? SignUpDate { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please acknowledge the \"Totally Binding Commitment\".")]
        public bool IsAcknowledged { get; set; }



    }
}