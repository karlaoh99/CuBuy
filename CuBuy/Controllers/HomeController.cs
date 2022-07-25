using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CuBuy.Models;
using System.Threading.Tasks;
using CuBuy.Data;
using System.Collections.Generic;
using System;

namespace CuBuy.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(UserManager<AppUser> userManager)
        {
        }

        public IActionResult Index() => View();
    }
}