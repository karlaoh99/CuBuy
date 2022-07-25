using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using CuBuy.Models;

namespace CuBuy.ViewModels
{
    public class ListAdsModel : ListModel
    {
        public IEnumerable<Ad> Ads {get; set;}
    }

    public class AdInsertModel
    {
        [Required(ErrorMessage = "Enter the product of the ad")]
        public string Product { get; set; }

        [Required(ErrorMessage = "Enter the price of the ad")]
        [Range(0.01, 10e8)]
        public float Price { get; set; }

        [Required(ErrorMessage= "Enter how many units you have")]
        [Range(1, 1000000)]
        public int UnitsAvailable{ get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Choose the category of the product")]
        public string Category { get; set; }
    }

    public class BuyProductModel
    {
        public Ad Ad {get; set;}
        public int Units { get; set;}

        [Required(ErrorMessage="Please enter a credit card number")]
        [RegularExpression("[0-9]+", ErrorMessage="Please enter only numbers")]
        public string CreditCard { get; set; }

        public string ReturnUrl {get; set;}
    }
}
