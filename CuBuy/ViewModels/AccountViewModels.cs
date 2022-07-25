using System.ComponentModel.DataAnnotations;

namespace CuBuy.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage="Enter your email")]
        [EmailAddress]
        public string Email {get; set;}

        [Required(ErrorMessage="Enter your password")]
        public string Password {get; set;}
    }   

    public class SignUpModel
    {
        [Required(ErrorMessage="Enter a name")]
        public string Name {get; set;}

        [Required(ErrorMessage="Enter an email")]
        [EmailAddress]
        public string Email {get; set;}

        [Required(ErrorMessage="Enter a password")]
        public string Password {get; set;}
    }

    public class UpgradeAccountModel
    {
        [Required(ErrorMessage = "Please enter a credit card number")]
        [RegularExpression("[0-9]+", ErrorMessage = "Please enter only numbers")]
        public string CreditCard { get; set; }
        public string Password {get; set;}
    }
}