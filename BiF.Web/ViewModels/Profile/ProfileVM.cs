
using System;
using System.ComponentModel.DataAnnotations;

namespace BiF.Web.ViewModels
{
    public class ProfileVM {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Zip { get; set; }

        [Required]
        public string Phone { get; set; }
        public string Email { get; set; }

        [Required]
        public string RedditUsername { get; set; }
        public string UntappdUsername { get; set; }
        public string References { get; set; }
        [Required]
        public string Wishlist { get; set; }
        public string Comments { get; set; }

        [Required]
        public int? Piney { get; set; }
        [Required]
        public int? Juicy { get; set; }
        [Required]
        public int? Tart { get; set; }
        [Required]
        public int? Funky { get; set; }
        [Required]
        public int? Malty { get; set; }
        [Required]
        public int? Roasty { get; set; }
        [Required]
        public int? Sweet { get; set; }
        [Required]
        public int? Smokey { get; set; }
        [Required]
        public int? Spicy { get; set; }
        [Required]
        public int? Crisp { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }


    }
}