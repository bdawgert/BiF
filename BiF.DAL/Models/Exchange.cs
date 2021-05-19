using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{
    public class Exchange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Theme { get; set; }
        [MaxLength(4000)]
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        [ForeignKey("Creator")]
        [MaxLength(36)]
        public string CreatorId { get; set; }
        public DateTime? UpdateDate { get; set; }
        [ForeignKey("Updater")]
        [MaxLength(36)]
        public string UpdaterId { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public DateTime? MatchDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public bool PrivateMatches { get; set; }
        public bool Deleted { get; set; }

        public double? MinOunces { get; set; }
        public double? MinRating { get; set; }
        public double? MinCost { get; set; }
        public double? MinBoxRating { get; set; }
        public int? MinUnique { get; set; }

        public virtual ICollection<SignUp> SignUps { get; set; }
        public virtual ICollection<Match> Matches { get; set; }
        public virtual IdentityUser Creator { get; set; }
        public virtual IdentityUser Updater { get; set; }

    }
}
