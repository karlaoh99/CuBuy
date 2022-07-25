using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CuBuy.Infraestructure;
using CuBuy.Models;
using CuBuy.Data;
using System;
using CuBuy.ViewModels;

namespace CuBuy.Controllers
{
    public class CartController : Controller
    {
        private IAdRepository adRepository;

        public CartController(IAdRepository adRepository) => this.adRepository = adRepository;

        public ViewResult Index(string returnUrl)
        {
            return View(new CartViewModel{ Cart = GetCart(), ReturnUrl=returnUrl });
        }

        public RedirectToActionResult AddToCart(Guid id, int units, string returnUrl)
        {
            var ad = adRepository.GetAd(id);

            if (ad != null)
            {
                Cart cart = GetCart();
                cart.AddItem(ad, units);
                SaveCart(cart);
            }
            return RedirectToAction("Index", new { returnUrl});
        }

        public RedirectToActionResult RemoveFromCart(Guid id, string returnUrl)
        {
            var ad = adRepository.GetAd(id);

            if (ad != null)
            {
                Cart cart = GetCart();
                cart.RemoveLine(ad);
                SaveCart(cart);
            }
            return RedirectToAction("Index", new {returnUrl});
        }

        public RedirectToActionResult SetLineUnits(Guid id, int units)
        {
            var ad = adRepository.GetAd(id);
            var cart = GetCart();
            cart.SetLineUnits(ad, units);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public RedirectToActionResult Clear()
        {
            var cart = GetCart();
            cart.Clear();
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        private Cart GetCart() =>  HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart(); 
        
        private void SaveCart(Cart cart) => HttpContext.Session.SetJson("Cart", cart);
    }
}