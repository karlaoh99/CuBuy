using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CuBuy.Models;
using System;
using CuBuy.Data;

namespace CuBuy.Data
{
    public interface IPurchaseRepository
    {
        IEnumerable<Purchase> Purchases { get; }
        Purchase GetPurchase(Guid id);
        bool AddPurchase(Purchase purchase);
        bool RemovePurchase(Purchase purchase);
        IEnumerable<Purchase> GetByUser(string id);
    }

    public class PurchaseRepositoryEF : IPurchaseRepository
    {
        private AppDbContext context;

        public PurchaseRepositoryEF(AppDbContext context) => this.context = context;

        public IEnumerable<Purchase> Purchases => context.Purchases;
        
        public Purchase GetPurchase(Guid id)
        {
            var purchase = context.Purchases.Find(id);
            context.Entry(purchase).Reference(p => p.Ad).Load();
            return purchase;
        }

        public bool AddPurchase(Purchase purchase)
        {
            context.Purchases.Add(purchase);
            return context.SaveChanges() > 0;
        }

        public bool RemovePurchase(Purchase purchase)
        {
            context.Purchases.Remove(purchase);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<Purchase> GetByUser(string id)
        {
            return context.Purchases.Where(p => p.Buyer.Id == id).Include(p=>p.Ad).Include(p=>p.Ad.User).ToArray();
        }
    }
}