using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BiF.DAL.Models
{
    public class MatchPreference
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        [MaxLength(36)]
        public string UserId { get; set; }

        public MatchPreferenceType PreferenceType { get; set; }
        public string Value { get; set; }

        public virtual IdentityUser User { get; set; }

    }

    public enum MatchPreferenceType
    {
        NotUser = -1,
        User = 1,
        NotState = -2,
        State = 2,
        Distance = 3
    }

}
