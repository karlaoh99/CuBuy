using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CuBuy.Models;
using CuBuy.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace CuBuy.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private RoleManager<IdentityRole> roleManager;
        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        #region LISTING
        public IActionResult Users()
        { 
             return View(userManager.Users.ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> Users(string role, string status)
        {   
            IEnumerable<AppUser> users;
            if (role != "All")
                users = await userManager.GetUsersInRoleAsync(role);
            else
                users = userManager.Users.ToArray();
            return View(users);
        }

        #endregion

        #region ADD
        public IActionResult AddUser() => View();

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser{UserName=model.Name, Email=model.Email};
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.Admin)
                        await userManager.AddToRoleAsync(user, "Admin");
                    if (model.Seller)
                        await userManager.AddToRoleAsync(user, "Seller");
                    if (model.Buyer)
                        await userManager.AddToRoleAsync(user, "Buyer");

                    return RedirectToAction("Users"); 
                } else {
                    foreach (IdentityError error in result.Errors) {
                        ModelState.AddModelError("", error.Description);
                    }  
                }
            }
            return View(model);
        }

        #endregion

        #region EDIT

        public async Task<IActionResult> Edit(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var model = new EditUserModel{Name=user.UserName, Email=user.Email};
            model.Admin = await userManager.IsInRoleAsync(user, "Admin");
            model.Seller = await userManager.IsInRoleAsync(user, "Seller");
            model.Buyer = await userManager.IsInRoleAsync(user, "Buyer");
            return View(model);
        }

        #endregion

        #region DELETE
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded) {
                    return RedirectToAction("Users");
                } else {
                    AddErrorFromResults(result);
                }
            } else {
                ModelState.AddModelError("", "User Not Found");
            }
            return RedirectToAction("Users");
        }

        #endregion

        private void AddErrorFromResults(IdentityResult result){
            foreach(var e in result.Errors)
                ModelState.AddModelError("", e.Description);
        }
    }
}