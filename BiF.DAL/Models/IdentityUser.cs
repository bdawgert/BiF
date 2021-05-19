using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{
    public class IdentityUser
    {
        public IdentityUser() { 
            Claims = new List<IdentityClaim>();
            Roles = new List<IdentityRole>();
        }

        [Key]
        [MaxLength(36)]
        public string Id { get; set; }
        [MaxLength(20)]
        public string UserName { get; set; }
        [MaxLength(64)]
        public string Email { get; set; }
        [MaxLength(32)]
        public byte[] Entropy { get; set; }
        [MaxLength(128)]
        public string PasswordHash { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public bool Approved { get; set; }
        public UserStatuses UserStatus { get; set; }
        public DateTime? LastLogin { get; set; }

        public virtual ICollection<IdentityClaim> Claims { get; set; }
        public virtual ICollection<IdentityRole> Roles { get; set; }
        public virtual Profile Profile { get; set; }

        [ForeignKey("SenderId")]
        public virtual ICollection<Match> SendingMatches { get; set; }
        [ForeignKey("RecipientId")]
        public virtual ICollection<Match> ReceivingMatches { get; set; }

        public virtual ICollection<SignUp> SignUps { get; set; }
        public virtual ICollection<MatchPreference> MatchPreferences { get; set; }

        public enum UserStatuses
        {
            NotApproved = -1,
            None = 0,
            Approved = 1,
            Deleted = -99

        }

    }
}