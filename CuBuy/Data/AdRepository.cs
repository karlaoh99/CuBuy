using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CuBuy.Models;
using System;
using CuBuy.Data;

namespace CuBuy.Data
{
    public interface IAdRepository
    {
        IEnumerable<Ad> Ads { get; }
        Ad GetAd(Guid id);
        bool Add(Ad ad);
        bool Remove(Guid id);
        bool Update(Ad ad);
        IEnumerable<Ad> GetAdsByUser(string id);
    }

    public class AdRepositoryEF : IAdRepository
    {
        private AppDbContext context;

        public AdRepositoryEF(AppDbContext context) => this.context = context;

        public IEnumerable<Ad> Ads => context.Ads.Where(a => a.User != null);

        public bool Add(Ad ad)
        {
            context.Ads.Add(ad);
            return context.SaveChanges() > 0;
        }

        public Ad GetAd(Guid id)
        {
            var ad = context.Ads.Find(id);
            context.Entry(ad).Reference(a => a.User).Load();
            return ad;
        }

        public IEnumerable<Ad> GetAdsByUser(string id)
        {
            return context.Ads.Where(a => a.User != null && a.User.Id == id);
        }

        public bool Remove(Guid id)
        {
            var old = context.Ads.Find(id);
            context.Entry(old).Reference(a => a.User).Load();
            old.User = null;
            return context.SaveChanges() > 0;
        }

        public bool Update(Ad ad)
        {
            var old = context.Ads.Find(ad.Id);
            old.Product = ad.Product;
            old.Price = ad.Price;
            old.Description = ad.Description;
            old.UnitsAvailable = ad.UnitsAvailable;
            return context.SaveChanges() > 0;
        }
    }
}