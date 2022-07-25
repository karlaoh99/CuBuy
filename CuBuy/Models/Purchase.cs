using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuBuy.Models;

namespace CuBuy.Models
{
    public class Purchase
    {
        public Guid Id {get; set;}
        public AppUser Buyer {get; set;}
        public Ad Ad { get; set; }
        public int Units { get; set; }
        public DateTime DateTime {get; set;}
    }
}
