using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiF.DAL.Models
{
    public class IdentityRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(36)]
        public string UserId { get; set; }
        [MaxLength(32)]
        public string Name { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser Users { get; set; }

    }
}
