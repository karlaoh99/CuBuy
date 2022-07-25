using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CuBuy.Models;
using System;
using CuBuy.Data;

namespace CuBuy.Data
{
    public interface IAuctionRepository
    {
        IEnumerable<Auction> Auctions { get; }
        Auction GetAuction(Guid id);
        bool Add(Auction auction);
        bool Remove(Guid id);
        bool Finish(Guid id);
        bool Update(Guid id, AppUser user, float currentBiggestPrice);
        IEnumerable<Auction> GetAuctionsByUser(string id);
    }

    public class AuctionRepositoryEF : IAuctionRepository
    {
        private AppDbContext context;

        public AuctionRepositoryEF(AppDbContext context) => this.context = context;

        public IEnumerable<Auction> Auctions => context.Auctions.Where(a => a.Seller != null && !a.ProductSold);

        public Auction GetAuction(Guid id)
        {
            var auction = context.Auctions.Find(id);
            context.Entry(auction).Reference(auction => auction.Seller).Load();
            context.Entry(auction).Reference(auction => auction.Buyer).Load();
            return auction;
        }

        public bool Add(Auction auction)
        {
            context.Auctions.Add(auction);
            return context.SaveChanges() > 0;
        }

        public bool Remove(Guid id)
        {
            var toRemove = context.Auctions.Find(id);
            context.Entry(toRemove).Reference(auction => auction.Seller).Load();
            toRemove.Seller = null;
            return context.SaveChanges() > 0;
        }

        public bool Finish(Guid id)
        {
            var toRemove = context.Auctions.Find(id);
            toRemove.ProductSold = true;
            return context.SaveChanges() > 0;
        }

        public bool Update(Guid id, AppUser currentBuyer, float currentBiggestPrice)
        {
            var toUpdate = context.Auctions.Find(id);
            toUpdate.Buyer = currentBuyer;
            toUpdate.FinalPrice = currentBiggestPrice;
            return context.SaveChanges() > 0;
        }

        public IEnumerable<Auction> GetAuctionsByUser(string id)
        {
            return context.Auctions.Where(a => a.Seller != null && a.Seller.Id == id).Include("Buyer");
        }
    }
}