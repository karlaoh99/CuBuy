using CuBuy.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CuBuy.Components
{
    public class EditLineUnitsForm: ViewComponent
    {
        public IViewComponentResult Invoke(CartLine line)
        {
            return View( line );
        }
    }
}