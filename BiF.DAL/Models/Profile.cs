using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{
    public class Profile
    {
        [Key]
        [ForeignKey("User")]
        [MaxLength(36)]
        public string Id { get; set; }

        [MaxLength(32)]
        public string FullName { get; set; }

        [MaxLength(64)]
        public string Address { get; set; }
        [MaxLength(64)]
        public string City { get; set; }
        [MaxLength(2), MinLength(2)]
        public string State { get; set; }
        [MaxLength(10), MinLength(5)]
        public string Zip { get; set; }
        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        [MaxLength(32)]
        public string RedditUsername { get; set; }
        [MaxLength(32)]
        public string UntappdUsername { get; set; }
        [MaxLength(4000)]
        public string References { get; set; }
        [MaxLength(4000)]
        public string Wishlist { get; set; }
        [MaxLength(4000)]
        public string Comments { get; set; }
        [MaxLength(4000)]
        public string DeliveryNotes { get; set; }

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

        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        
        public virtual IdentityUser User { get; set; }

    }
}
