
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiF.Web.ViewModels
{
    public class ProfileVM {
        public string Id { get; set; }

        [Required(ErrorMessage = "Full Name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Street Address is Required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "City is Required")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is Required")]
        public string State { get; set; }
        [Required(ErrorMessage = "Zip Code is Required"), MaxLength(5, ErrorMessage = "Zip Code Must Be Five Digits"), MinLength(5, ErrorMessage = "Zip Code Must Be Five Digits"), RegularExpression(@"\d{5}", ErrorMessage = "Zip Code Must Be Digits")]
        public string Zip { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Reddit Username is Required")]
        public string RedditUsername { get; set; }
        public string UntappdUsername { get; set; }

        [Required(ErrorMessage = "Provide a brief list of prior beer exchange experience or references")]
        public string References { get; set; }
        //public string Wishlist { get; set; }
        public string Comments { get; set; }
        public string DeliveryNotes { get; set; }

        public Dictionary<string, int?> Flavors =>
            new Dictionary<string, int?> {
                {"Piney", Piney},
                {"Juicy", Juicy},
                {"Tart", Tart},
                {"Funky", Funky},
                {"Malty", Malty},
                {"Roasty", Roasty},
                {"Sweet", Sweet},
                {"Smokey", Smokey},
                {"Spicy", Spicy},
                {"Crisp", Crisp}
            };

        [Required(ErrorMessage = "Select a Preference for Piney Flavors")]
        public int? Piney { get; set; }
        [Required(ErrorMessage = "Select a Preference for Juicy Flavors")]
        public int? Juicy { get; set; }
        [Required(ErrorMessage = "Select a Preference for Tart Flavors")]
        public int? Tart { get; set; }
        [Required(ErrorMessage = "Select a Preference for Funky Flavors")]
        public int? Funky { get; set; }
        [Required(ErrorMessage = "Select a Preference for Malty Flavors")]
        public int? Malty { get; set; }
        [Required(ErrorMessage = "Select a Preference for Roasty Flavors")]
        public int? Roasty { get; set; }
        [Required(ErrorMessage = "Select a Preference for Sweet Flavors")]
        public int? Sweet { get; set; }
        [Required(ErrorMessage = "Select a Preference for Smokey Flavors")]
        public int? Smokey { get; set; }
        [Required(ErrorMessage = "Select a Preference for Spicy Flavors")]
        public int? Spicy { get; set; }
        [Required(ErrorMessage = "Select a Preference for Crisp Flavors")]
        public int? Crisp { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        //[Required(ErrorMessage = "Please confirm your commitment to the Beer It Forward rules.")]
        //[Range(typeof(bool), "true", "true", ErrorMessage = "Please confirm your commitment to the Beer It Forward rules.")]
        //public bool? IsSignedUp { get; set; }

    }
}