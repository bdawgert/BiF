using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{
    public class Match
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ExchangeId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public DateTime? MatchDate { get; set; }
        public DateTime? ShipDate { get; set; }

        [ForeignKey("ExchangeId")]
        public virtual Exchange Exchange { get; set; }

        [ForeignKey("SenderId")]
        public virtual BifIdentityUser Sender { get; set; }

        [ForeignKey("RecipientId")]
        public virtual BifIdentityUser Recipient { get; set; }
    }
}
