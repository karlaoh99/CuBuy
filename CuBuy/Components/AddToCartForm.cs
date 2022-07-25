using CuBuy.Data;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CuBuy.Components
{
    public class AddToCartForm : ViewComponent
    {
        private IAdRepository adRepository;

        public AddToCartForm(IAdRepository adRepository) => this.adRepository = adRepository;

        public IViewComponentResult Invoke(Guid productId)
        {
            return View( adRepository.GetAd(productId) );
        }
    }
}