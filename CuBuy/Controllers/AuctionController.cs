using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using CuBuy.Models;
using CuBuy.Data;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using CuBuy.Infraestructure;
using CuBuy.ViewModels;

namespace CuBuy.Controllers
{
    public class AuctionController : Controller
    {
        private IAuctionRepository auctionRepository;
        private IAdRepository adRepository;
        private INotificationRepository notificationRepository;
        private UserManager<AppUser> userManager;
        private IUrlHelperFactory urlHelperFactory;
        private readonly int pageSize = 10;

        public AuctionController(UserManager<AppUser> userManager, IUrlHelperFactory urlHelperFactory, IAuctionRepository auctionRepository, IAdRepository adRepository, INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
            this.adRepository = adRepository;
            this.auctionRepository = auctionRepository;
            this.userManager = userManager;
            this.urlHelperFactory = urlHelperFactory;
        }

        // Show all auctions
        public ViewResult Index(ListAuctionsModel model)
        {
            var auctions = auctionRepository.Auctions.OrderBy(time => time.StartTimeAuction)
                .Where(auction =>
                {
                    auction = auctionRepository.GetAuction(auction.Id);
                    return (model.MinPrice ?? 0) <= auction.FinalPrice && auction.FinalPrice <= (model.MaxPrice ?? float.MaxValue) && (model.Category ?? auction.Category) == auction.Category;
                });

            if (model.CurrentPage == 0)
            {
                model.CurrentPage = 1;
                model.TotalPages = (int)Math.Ceiling((decimal)auctions.Count() / pageSize);
            }

            model.Auctions = auctions.Skip((model.CurrentPage - 1) * pageSize).Take(pageSize);

            return View(model);
        }

        // Make a bid
        public async Task<IActionResult> MakeABid(Guid id, float price, string returnUrl)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            auctionRepository.Update(id, user, price);

            return Redirect(returnUrl);
        }

        // Show all auctions of an user
        public IActionResult ShowUserAuctions()
        {
            string userId = userManager.GetUserId(HttpContext.User);
            
            return View(auctionRepository.GetAuctionsByUser(userId).OrderBy(time => time.StartTimeAuction));
        }

        [HttpPost]
        public IActionResult ShowUserAuctions(string category)
        {
            string userId = userManager.GetUserId(HttpContext.User);
            
            IEnumerable<Auction> result;
            if (category != null)
                result = auctionRepository.GetAuctionsByUser(userId).Where(a => a.Category == category);
            else result = auctionRepository.GetAuctionsByUser(userId);
                        
            return View(result.OrderBy(time => time.StartTimeAuction));
        }

        // Insert a new auction
        public IActionResult InsertAuction() => View();

        [HttpPost]
        public async Task<IActionResult> InsertAuction(AuctionInsertModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                Auction auction = new Auction
                {
                    Product = model.Product,
                    Description = model.Description,
                    Category = model.Category,
                    Seller = user,
                    StartPrice = model.Price,
                    FinalPrice = model.Price,
                    StartTimeAuction = DateTime.Now,
                    FinalTimeAuction = model.FinalDate
                };
                auctionRepository.Add(auction);

                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ControllerContext);
                notificationRepository.AddNotification(new Notification
                {
                    User = auction.Seller,
                    Message = $"Successful auction of {auction.Product}",
                    Link = urlHelper.Action("AddToAuctionsList", "Buyer"),
                    DateTime = DateTime.Now,
                    Type = NotificationType.Success
                });

                return RedirectToAction("ShowUserAuctions");
            }
            return View(model);
        }

        // Remove an Auction from The auctions list
        public RedirectToActionResult DeleteAuction(Guid id)
        {
            var toRemove = auctionRepository.GetAuction(id);
            if (toRemove != null)
                auctionRepository.Remove(id);

            return RedirectToAction("ShowUserAuctions");
        }


        //a particular auction view
        [HttpPost]
        public ViewResult AParticularAuctionIndex(Guid id)
        {
            Auction auction = auctionRepository.GetAuction(id);
            return View(auction);
        } 

        private void UpdateAuctionsList(Guid id, AppUser buyer, float price, bool productSold)
        {
            auctionRepository.Update(id, buyer, price);
        }

        // how auction works
        public async Task<IActionResult> AuctionProcess(Guid auctionId)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ControllerContext);
            
            Auction auction = auctionRepository.GetAuction(auctionId);
            DateTime currentTime = DateTime.Now;
            while((currentTime = DateTime.Now) < auction.FinalTimeAuction)
            {
                //Aki hay que ver como se reciben las entradas
                //float inputCount = input.Count();
                //AppUser inputUser = input.User(); // esto es como kien ponga pagar una cantidad mayor a la existente como guardarlo
                //if(inputCount > auction.FinalPrice)
                //{
                //    UpdateAuctionsList(auctionId, inputUser, inputCount);
                //}
            }

            if(auction.Buyer != null)
            {
                //auction.ProductSold=true;
                ////quitar el dinero de la tarjeta
                //notificationRepository.AddNotification(
                //new Notification {
                //    User = auction.Ad.User, 
                //    Message = $"Successful auction of {auction.Ad.Product}",
                //    Link= urlHelper.Action("AuctionProcess", "Buyer"),
                //    DateTime = DateTime.Now,
                //    Type = NotificationType.Success });
            }
            else
            {
                //new Notification {
                //    User = auction.Ad.User, 
                //    Message = $"Unsuccessful auction of {auction.Ad.Product}",
                //    Link= urlHelper.Action("AuctionProcess", "Buyer"),
                //    DateTime = DateTime.Now,
                //    Type = NotificationType.Error };
            }
            
            auctionRepository.Remove(auction.Id);
            return RedirectToAction("Index");
        }
    }
}