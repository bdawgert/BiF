using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{
    public class SignUp
    {
        [ForeignKey("Exchange")]
        [Key]
        [Column(Order = 0)]
        public int ExchangeId { get; set; }

        [ForeignKey("User")]
        [Key]
        [Column(Order = 1)]
        [MaxLength(36)]
        public string UserId { get; set; }

        public DateTime SignUpDate { get; set; }
        public bool Approved { get; set; }

        public virtual Exchange Exchange { get; set; }
        public virtual IdentityUser User { get; set; }


    }
}
