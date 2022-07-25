using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuBuy.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CuBuy.Models
{
    public class Auction
    {
        public Guid Id {get;set;}
        public string Product { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public float StartPrice {get;set;}
        public DateTime StartTimeAuction {get;set;}
        public bool ProductSold {get;set;}
        public float FinalPrice {get;set;}
        public AppUser Seller { get; set; }
        public AppUser Buyer {get;set;}
        public DateTime FinalTimeAuction {get;set;}
    }   
}
