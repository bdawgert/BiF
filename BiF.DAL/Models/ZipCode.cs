using System.ComponentModel.DataAnnotations;

namespace BiF.DAL.Models
{
    public class ZipCode
    {
        [Key]
        public string Zip { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}

