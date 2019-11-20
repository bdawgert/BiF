using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{

    public class Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("Exchange")]
        public int? ExchangeId { get; set; }
        [MaxLength(32)]
        public string Type { get; set; }
        public int? UntappdId { get; set; }
        [MaxLength(4000)]
        public string Name { get; set; }
        [MaxLength(16)]
        public string Format { get; set; }
        public double? USOunces { get; set; }
        public double? UntappdRating { get; set; }
        public double? Cost { get; set; }

        public virtual Exchange Exchange { get; set; }
        public virtual IdentityUser User { get; set; }

    }


}
