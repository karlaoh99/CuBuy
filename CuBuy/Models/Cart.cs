using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuBuy.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CuBuy.Models
{
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        public virtual void AddItem(Ad ad, int units)
        {
            var line = lineCollection.Where(a => a.Ad.Id == ad.Id).FirstOrDefault();
            if (line == null)
                lineCollection.Add(new CartLine {Ad = ad, Units = units});
            else line.Units = Math.Min(line.Units + units, ad.UnitsAvailable);
        }

        public virtual void SetLineUnits(Ad ad, int units)
        {
            var line = lineCollection.Where(a => a.Ad.Id == ad.Id).FirstOrDefault();
            if (line != null)
                line.Units = units;
        }

        public virtual void RemoveLine(Ad ad) => 
            lineCollection.RemoveAll(l => l.Ad.Id == ad.Id);
        
        public virtual float ComputeTotalValue() =>
            lineCollection.Sum(l => l.Ad.Price * l.Units);

        public virtual void Clear() => lineCollection.Clear();

        public virtual IEnumerable<CartLine> Lines => lineCollection;

    }

    public class CartLine
    {
        public Guid Id {get; set;}
        public Ad Ad {get; set;}
        public int Units {get; set;}
    }
}
