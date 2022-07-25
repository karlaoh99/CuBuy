using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Identity;
using CuBuy.Models;
using System.Linq;
using CuBuy.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CuBuy.Infraestructure
{
    [HtmlTargetElement("div", Attributes="not-user")]
    public class NotificationsTagHelpers : TagHelper
    {
        private INotificationRepository notificationRepository;
        private UserManager<AppUser> userManager;

        public NotificationsTagHelpers(UserManager<AppUser> userManager, INotificationRepository notificationRepository)
        {
            this.userManager = userManager;
            this.notificationRepository = notificationRepository;
        }
      
        [HtmlAttributeName("not-user")]
        public ClaimsPrincipal User {get; set;}
        public int ListSize {get; set;}

        public TagBuilder SuccessIcon 
        {
            get
            {
                TagBuilder iconBg = new TagBuilder("div");
                iconBg.AddCssClass("bg-c1 img-cir img-40");
                TagBuilder icon = new TagBuilder("i");
                icon.AddCssClass("fas fa-check");
                iconBg.InnerHtml.AppendHtml(icon);
                return iconBg;
            }
        }

        public TagBuilder WarningIcon 
        {
            get
            {
                TagBuilder iconBg = new TagBuilder("div");
                iconBg.AddCssClass("bg-c3 img-cir img-40");
                TagBuilder icon = new TagBuilder("i");
                icon.AddCssClass("fas fa-exclamation");
                iconBg.InnerHtml.AppendHtml(icon);
                return iconBg;
            }
        }

        public TagBuilder ErrorIcon 
        {
            get
            {
                TagBuilder iconBg = new TagBuilder("div");
                iconBg.AddCssClass("bg-c2 img-cir img-40");
                TagBuilder icon = new TagBuilder("i");
                icon.AddCssClass("fas fa-times");
                iconBg.InnerHtml.AppendHtml(icon);
                return iconBg;
            }
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            AppUser user = await userManager.GetUserAsync(User);
            if (user == null) return;
            var notifications = notificationRepository.GetByUser(user.Id).Take(ListSize);

            //building the title
            TagBuilder header = new TagBuilder("div");
            header.AddCssClass("notifi__title");
            TagBuilder title = new TagBuilder("p");
            title.InnerHtml.Append($"You have {notifications.Count()} notifications");
            header.InnerHtml.AppendHtml(title);
            output.Content.AppendHtml(header);

            //building the list
            foreach ( var n in notifications)
            {
                TagBuilder notificationTag = new TagBuilder("div");
                notificationTag.AddCssClass("notifi__item");

                TagBuilder notificationContent = new TagBuilder("div");
                notificationContent.AddCssClass("content");

                TagBuilder description = new TagBuilder("p");
                description.InnerHtml.Append(n.Message);

                TagBuilder date = new TagBuilder("span");
                date.AddCssClass("date");
                date.InnerHtml.Append(n.DateTime.ToString());

                notificationContent.InnerHtml.AppendHtml(description);
                notificationContent.InnerHtml.AppendHtml(date);
                switch (n.Type)
                {
                    case NotificationType.Success:
                        notificationTag.InnerHtml.AppendHtml(SuccessIcon);
                        break;
                    case NotificationType.Warning:
                        notificationTag.InnerHtml.AppendHtml(WarningIcon);
                        break;
                    case NotificationType.Error:
                        notificationTag.InnerHtml.AppendHtml(ErrorIcon);
                        break;
                }
                notificationTag.InnerHtml.AppendHtml(notificationContent);
                output.Content.AppendHtml(notificationTag);
            }
        }
    }
}