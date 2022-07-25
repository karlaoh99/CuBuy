using CuBuy.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using CuBuy.ViewModels;

namespace CuBuy.Components
{
    public class BuyProduct : ViewComponent
    {
        private IAdRepository adRepository;

        public BuyProduct(IAdRepository adRepository) => this.adRepository = adRepository;

        public IViewComponentResult Invoke(Guid productId)
        {
            return View( new BuyProductModel {Ad = adRepository.GetAd(productId), Units=1} );
        }
    }
}