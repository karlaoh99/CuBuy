using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;
using CuBuy.Models;
using CuBuy.ViewModels;

namespace CuBuy.Controllers
{
    /*This controller manages the requests associated with the users accounts
     (i.e. singin, login, logout, account administration,...)*/
    
    [Authorize]
    public class AccountController: Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private IUserValidator<AppUser> userValidator;
        private IPasswordValidator<AppUser> passwordValidator;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IPasswordHasher<AppUser> passwordHasher, IUserValidator<AppUser> userValidator, IPasswordValidator<AppUser> passwordValidator)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
            this.userValidator = userValidator;
            this.passwordValidator = passwordValidator;
        }
        
        public ActionResult Index() => View();
             
        #region SIGNUP

        [AllowAnonymous]
        public ViewResult Signup(string returnUrl) => View();
     
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Signup(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser{ UserName=model.Name, Email=model.Email };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Buyer");
                    await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    return Redirect(ViewBag.returnUrl ?? "/");
                } 
                else 
                {
                    foreach (IdentityError error in result.Errors) 
                    {
                        ModelState.AddModelError("", error.Description);
                    }  
                }
            }
            return View(model);
        }

        #endregion

        #region LOGIN

        [AllowAnonymous]
        public IActionResult Login(string returnUrl) => View();

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //Tries to find a user in the database with the email provided by the form
                AppUser user = await userManager.FindByEmailAsync(loginModel.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync(); //closes a possible previous open session
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
                    if (result.Succeeded)
                        return Redirect(returnUrl ?? "/");
                }
                ModelState.AddModelError("", "Invalid user or password");
            }
            return View(loginModel);
        }
      
        #endregion
        
        #region LOGOUT

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        // #region EDIT

        // public async Task<ViewResult> Edit()
        // {
        //     AppUser user = await userManager.GetUserAsync(HttpContext.User);
        //     return View(user);
        // }

        // [HttpPost]
        // public async Task<IActionResult> Edit(string name, string password, string bankAccount) 
        // {
        //     AppUser user = await userManager.GetUserAsync(HttpContext.User);
        //     if (user != null) {
        //         user.UserName = name;
        //         IdentityResult validPass = null;
        //         if (!string.IsNullOrEmpty(password)) {
        //             validPass = await passwordValidator.ValidateAsync(userManager, user, password);
        //             if (validPass.Succeeded) {
        //                 user.PasswordHash = passwordHasher.HashPassword(user, password);
        //             } else {
        //                 AddErrorFromResults(validPass);
        //             }
        //         }
        //         user.BankAccount = bankAccount;
        //         if (validPass == null || (password != string.Empty && validPass.Succeeded))
        //         {
        //             IdentityResult result = await userManager.UpdateAsync(user);
        //             if (result.Succeeded){
        //                 return RedirectToAction("Manager");
        //             } else {
        //                 AddErrorFromResults(result);
        //             }
        //         } 
        //     } else {
        //         ModelState.AddModelError("", "User Not Found");
        //     }
        //     return View(user);
        // }

        // #endregion

        #region UPGRADE ACCOUNT

        [HttpPost]
        public async Task<IActionResult> UpgradeAccount(UpgradeAccountModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                if (userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                {
                    await signInManager.SignOutAsync();
                    await userManager.AddToRoleAsync(user, "Seller");
                    await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                }
            }
            return RedirectToAction("Index", "Home");


        }

        #endregion

        private void AddErrorFromResults(IdentityResult result)
        {
            foreach(var e in result.Errors)
                ModelState.AddModelError("", e.Description);
        } 
    }
}