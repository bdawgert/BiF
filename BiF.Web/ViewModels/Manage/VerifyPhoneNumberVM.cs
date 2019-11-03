using System.ComponentModel.DataAnnotations;

namespace BiF.Web.ViewModels.Manage
{
    public class VerifyPhoneNumberVM
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}