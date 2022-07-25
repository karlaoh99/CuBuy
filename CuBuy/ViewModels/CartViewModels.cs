using CuBuy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CuBuy.ViewModels
{
    public class CartViewModel
    {
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; }

        [Required(ErrorMessage = "Please enter a credit card number")]
        [RegularExpression("[0-9]+", ErrorMessage = "Please enter only numbers")]
        public string CreditCard { get; set; }
    }
}
