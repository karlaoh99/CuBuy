using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Razor.TagHelpers;
using CuBuy.Models;
using System.Threading.Tasks;


namespace CuBuy.Infraestructure
{
    [HtmlTargetElement("td", Attributes = "user-roles")]
    public class UserRolesTagHelper : TagHelper
    {
        private UserManager<AppUser> userManager;

        public UserRolesTagHelper(UserManager<AppUser> userManager) => this.userManager = userManager;

        [HtmlAttributeName("user-roles")]
        public string UserId {get; set; }
       
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            AppUser user = await userManager.FindByIdAsync(UserId);
            if (user != null){
                var roles = await userManager.GetRolesAsync(user);
                string encoded = "";
                foreach (var r in roles)
                    if (r == "Admin")
                        encoded += $"<span class=\"role admin\">{r}</span><span> </span>";
                    else if (r == "Buyer")
                        encoded += $"<span class=\"role user\">{r}</span><span> </span>";
                    else
                        encoded += $"<span class=\"role member\">{r}</span><span> </span>";
                output.Content.SetHtmlContent(encoded);
            } else {
                output.Content.SetHtmlContent($"<span class=\"badge badge-danger\">No Roles</span>");
            }
        }
    }
}