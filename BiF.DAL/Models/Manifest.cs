using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiF.DAL.Models
{
    public class Manifest
    {

        public int Id { get; set; }
        public int ExchangeId { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int? UtappdId { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public double USOunces { get; set; }
        public double UntappdRating { get; set; }
    }


}
