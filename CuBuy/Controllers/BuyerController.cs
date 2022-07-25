using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuBuy.Infraestructure;
using CuBuy.Data;
using CuBuy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CuBuy.Controllers
{
    [Authorize(Roles="Buyer")]
    public class BuyerController : Controller
    {
        private IPurchaseRepository purchaseRepository;
        private UserManager<AppUser> userManager;
        private IAdRepository adRepository;
        private INotificationRepository notificationRepository;
        private IUrlHelperFactory urlHelperFactory;

        public BuyerController(IPurchaseRepository purchaseRepository, UserManager<AppUser> userManager, IAdRepository adRepository, INotificationRepository notificationRepository, IUrlHelperFactory urlHelperFactory)
        {
            this.purchaseRepository = purchaseRepository;
            this.userManager = userManager;
            this.adRepository = adRepository;
            this.notificationRepository = notificationRepository;
            this.urlHelperFactory = urlHelperFactory;
        }

        public ViewResult Index()
        {
            return View(purchaseRepository.Purchases);
        }

        public IActionResult ListPurchases()
        {
            return View(purchaseRepository.Purchases);
        }

        public async Task<IActionResult> ShowPurchases()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            return View(purchaseRepository.GetByUser(user.Id));
        }

        [HttpPost]
        public async Task<IActionResult> ShowPurchases(string seller_name, DateTime? date, float? price)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            IEnumerable<Purchase> filtered = purchaseRepository.Purchases.Where(x => x.Buyer.Id == user.Id &&
                (seller_name == null || x.Ad.User.UserName == seller_name) &&
                (date.Equals(null) || x.DateTime.Equals(date)) &&
                (price == null || x.Ad.Price.Equals(price)));
            return View(filtered);
        }

        public void AddPurchase(AppUser buyer, Ad ad, int units)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ControllerContext);
            Purchase purchase = new Purchase {Buyer = buyer, Ad = ad, Units=units, DateTime = DateTime.Now };
            purchaseRepository.AddPurchase(purchase);
            
            ad.UnitsAvailable -= units;
            adRepository.Update(ad);

            notificationRepository.AddNotification(
                new Notification {
                    User = buyer, 
                    Message = $"Successfuly purchased {units} unit(s) of {ad.Product}",
                    Link= urlHelper.Action("ShowPurchases", "Buyer"),
                    DateTime = DateTime.Now,
                    Type = NotificationType.Success });
            
            notificationRepository.AddNotification(
                new Notification {
                    User = ad.User,
                    Message = $"{units} unit(s) of {ad.Product} have been sold",
                    Link=urlHelper.Action("ShowSales", "Seller"),
                    DateTime = DateTime.Now,
                    Type = NotificationType.Success
                });

            if (ad.UnitsAvailable == 0)
                notificationRepository.AddNotification(
                new Notification {
                    User = ad.User,
                    Message = $"All units of {ad.Product} have been sold, do you have more?",
                    Link=urlHelper.Action("EditAd", "Seller", new {id =ad.Id}),
                    DateTime = DateTime.Now,
                    Type = NotificationType.Warning
                });
        }

        public async Task<IActionResult> Buy(Guid id, int units, long creditCard, string returnUrl)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var ad = adRepository.GetAd(id);
            if (BankSystem.Pay(creditCard))
                AddPurchase(user, ad, units);
            else notificationRepository.AddNotification(
                new Notification {
                    User = user,
                    Message = $"Sorry, there was problem with your credit card while paying for {units} unit(s) of {ad.Product}",
                    DateTime = DateTime.Now,
                    Type = NotificationType.Error        
                });
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> BuyCart()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var cart = GetCart();
            foreach (var line in cart.Lines){
                var ad = adRepository.GetAd(line.Ad.Id);
                AddPurchase(user, ad, line.Units);
            }
            cart.Clear();
            SaveCart(cart);
            return RedirectToAction("Index", "Ad");
        }

        private Cart GetCart() =>  HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();   
        
        private void SaveCart(Cart cart) => HttpContext.Session.SetJson("Cart", cart);

        public bool Pay(string creditCard)
        {
            Random r = new Random();
            return r.Next(100) % 100 != 0;
        }       
    }
}

