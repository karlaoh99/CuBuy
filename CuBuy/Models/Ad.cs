using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using CuBuy.Models;

namespace CuBuy.Models
{
    public class Ad
    {
        public Guid Id {get; set;}

        [Required(ErrorMessage = "Enter the product of the ad")]
        public string Product { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Enter the price of the ad")]
        [Range(0.01, 10e8)]
        public float Price { get; set; }
        
        [Required(ErrorMessage = "Choose the category of the product")]
        public string Category { get; set; }

        public int UnitsAvailable {get; set;}
        public DateTime DateTime { get; set; }
        public AppUser User { get; set; }
    }
}