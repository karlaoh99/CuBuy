using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using CuBuy.Data;
using System;
using CuBuy.ViewModels;

namespace CuBuy.Controllers
{
    public class AdController : Controller
    {
        private IAdRepository adRepository;
        private readonly int pageSize = 10;  

        public AdController(IAdRepository adRepository)
        {
            this.adRepository = adRepository;
        }

        public IActionResult Index(ListAdsModel model)
        {
            var ads = adRepository.Ads.OrderByDescending(ad => ad.DateTime.Date)
                .Where(ad => (model.MinPrice ?? 0) <= ad.Price && ad.Price <= (model.MaxPrice ?? float.MaxValue) && (model.Category ?? ad.Category) == ad.Category);
           
            if (model.CurrentPage == 0)
            {
                model.CurrentPage = 1;
                model.TotalPages = (int)Math.Ceiling((decimal)ads.Count()/pageSize);
            }
           
            model.Ads = ads.Skip((model.CurrentPage - 1) * pageSize).Take(pageSize);

            return View(model);
        }
                
        public IActionResult ShowAd(Guid id)
        {
            var ad = adRepository.GetAd(id);
            return View(ad);
        }
    }
}