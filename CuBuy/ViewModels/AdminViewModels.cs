using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CuBuy.Models
{
    public class AddUserModel
    {
        [Required(ErrorMessage="Please insert a name")]
        public string Name {get; set;}

        [Required(ErrorMessage="Please insert an email")]
        [EmailAddress]
        public string Email {get; set;}

        [Required(ErrorMessage="Please insert a password")]
        public string Password {get; set;}

        public bool Admin {get; set;}
        public bool Buyer {get; set;}
        public bool Seller {get; set;}
    }

    public class EditUserModel
    {
        [Required(ErrorMessage="Please insert a name")]
        public string Name {get; set;}

        [Required(ErrorMessage="Please insert an email")]
        [EmailAddress]
        public string Email {get; set;}

        public bool Admin {get; set;}
        public bool Buyer {get; set;}
        public bool Seller {get; set;}
    }
}