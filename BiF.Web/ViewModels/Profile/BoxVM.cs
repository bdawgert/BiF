
using System.Collections.Generic;

namespace BiF.Web.ViewModels.Profile
{
    public class BoxVM {
        public string UserId { get; set; }
        public int ExchangeId { get; set; }
        public string ExchangeName { get; set; }
        public List<BoxItem> Items { get; set; }
    }

    public class BoxItem
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int? UntappdId { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public double? USOunces { get; set; }
        public double? UntappdRating { get; set; }
        public double? Cost { get; set; }
    }

}