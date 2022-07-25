using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CuBuy.Data;
using CuBuy.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;
using CuBuy.ViewModels;

namespace CuBuy.Controllers
{
    public class SellerController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAdRepository adRepository;
        private IPurchaseRepository purchaseRepository;

        public SellerController(UserManager<AppUser> userManager, IAdRepository adRepository, IPurchaseRepository purchaseRepository)
        {
            this.userManager = userManager;
            this.adRepository = adRepository;
            this.purchaseRepository = purchaseRepository;
           
        }

        public IActionResult ShowAds()
        {
            string userId = userManager.GetUserId(HttpContext.User);
            return View(adRepository.GetAdsByUser(userId).OrderByDescending(a => a.DateTime));
        }

        [HttpPost]
        public IActionResult ShowAds(string category)
        {
            string userId = userManager.GetUserId(HttpContext.User);
            IEnumerable<Ad> result;
            if (category != null)
                result = adRepository.GetAdsByUser(userId).Where(a => a.Category == category);
            else result = adRepository.GetAdsByUser(userId);
            return View(result.OrderByDescending(a => a.DateTime));
        }

        public IActionResult InsertAd() => View();

        [HttpPost]
        public async Task<IActionResult> InsertAd(AdInsertModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                Ad ad = new Ad
                {
                    Product = model.Product,
                    Description = model.Description,
                    Price = model.Price,
                    Category = model.Category,
                    UnitsAvailable = model.UnitsAvailable,
                    User = user,
                    DateTime = DateTime.Now
                };
                adRepository.Add(ad);
                return RedirectToAction("ShowAds");
            }
            return View(model); // retornar la vista del anuncio 
        }

        public IActionResult EditAd(Guid id)
        {
            var ad = adRepository.GetAd(id);
            return View(ad);
        }

        [HttpPost]
        public IActionResult EditAd(Ad model)
        {
            if (ModelState.IsValid)
            {
                adRepository.Update(model);
                return RedirectToAction("ShowAds");
            }
            return View(model); // retornar la vista del anuncio 
        }

        public IActionResult DeleteAd(Guid id)
        {
            adRepository.Remove(id);
            return RedirectToAction("ShowAds");
        }

        public async Task<IActionResult> ShowSales()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var filtered = new List<Purchase>(purchaseRepository.Purchases).FindAll(x => {
                var p = purchaseRepository.GetPurchase(x.Id);
                var a = adRepository.GetAd(p.Ad.Id);
                return a.User.Id == user.Id;
            });
            return View(filtered);
        }

        private void AddErrorFromResults(IdentityResult result)
        {
            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);
        }
    }
}
