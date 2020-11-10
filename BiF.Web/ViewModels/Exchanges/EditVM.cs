
using System;

namespace BiF.Web.ViewModels.Exchanges
{
    public class EditVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string Description { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public DateTime? MatchDate { get; set; }
        public DateTime? ShipDate { get; set; }

        public double MinOunces { get; set; }
        public double MinCost { get; set; }
        public double MinRating { get; set; }
    }
}