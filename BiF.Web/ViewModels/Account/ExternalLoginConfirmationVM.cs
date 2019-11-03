using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BiF.Web.ViewModels.Account
{
    public class ExternalLoginConfirmationVM
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}