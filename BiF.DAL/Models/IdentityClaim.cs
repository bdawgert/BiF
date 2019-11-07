using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{
    public class IdentityClaim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(36)]
        public string UserId { get; set; }
        [MaxLength(128)]
        public string Type { get; set; }
        [MaxLength(128)]
        public string Value { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser Users { get; set; }

    }
}
