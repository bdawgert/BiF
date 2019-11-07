using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{
    public class Match
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Exchange")]
        public int ExchangeId { get; set; }
        [ForeignKey("Sender")]
        [MaxLength(36)]
        public string SenderId { get; set; }
        [ForeignKey("Recipient")]
        [MaxLength(36)]
        public string RecipientId { get; set; }
        public DateTime? MatchDate { get; set; }
        public DateTime? ShipDate { get; set; }

        
        public virtual Exchange Exchange { get; set; }

        public virtual IdentityUser Sender { get; set; }
        public virtual IdentityUser Recipient { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
