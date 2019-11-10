using System.ComponentModel.DataAnnotations;

namespace BiF.Web.ViewModels.Manage
{
    public class ChangeEmailVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }


    }
}