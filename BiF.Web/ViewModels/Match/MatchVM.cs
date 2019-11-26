using System;
using System.Collections.Generic;

namespace BiF.Web.ViewModels.Match
{
    public class MatchVM
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string RedditUsername { get; set; }
        public string UntappdUsername { get; set; }

        public string References { get; set; }
        public string Comments { get; set; }
        public string DeliveryNotes { get; set; }

        public Dictionary<string, int?> Flavors =>
            new Dictionary<string, int?> {
                {"Piney", Piney},
                {"Juicy", Juicy},
                {"Tart", Tart},
                {"Funky", Funky},
                {"Malty", Malty},
                {"Roasty", Roasty},
                {"Sweet", Sweet},
                {"Smokey", Smokey},
                {"Spicy", Spicy},
                {"Crisp", Crisp}
            };

        public int? Piney { get; set; }
        public int? Juicy { get; set; }
        public int? Tart { get; set; }
        public int? Funky { get; set; }
        public int? Malty { get; set; }
        public int? Roasty { get; set; }
        public int? Sweet { get; set; }
        public int? Smokey { get; set; }
        public int? Spicy { get; set; }
        public int? Crisp { get; set; }

        public string SenderId { get; set; }
        public string Carrier { get; set; }
        public string TrackingNo { get; set; }
        public DateTime? ShipDate { get; set; }
    }
}
