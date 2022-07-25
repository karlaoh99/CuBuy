using CuBuy.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using CuBuy.ViewModels;

namespace CuBuy.Components
{
    public class MakeABid : ViewComponent
    {
        private IAuctionRepository auctionRepository;

        public MakeABid(IAuctionRepository auctionRepository) => this.auctionRepository = auctionRepository;

        public IViewComponentResult Invoke(Guid auctionId)
        {
            return View(new MakeABidModel { Auction = auctionRepository.GetAuction(auctionId) });
        }
    }
}