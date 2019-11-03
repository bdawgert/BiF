﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{
    public class Profile
    {
        [Key]
        [ForeignKey("User")]
        public string Id { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string RedditUsername { get; set; }
        public string UntappdUsername { get; set; }
        public string References { get; set; }
        public string Wishlist { get; set; }
        public string Comments { get; set; }

        public int? Piney { get; set; }
        public int? Juicey { get; set; }
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

        
        public virtual BifIdentityUser User { get; set; }

    }
}
