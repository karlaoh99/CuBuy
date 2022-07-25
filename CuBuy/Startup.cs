using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CuBuy.Data;
using CuBuy.Models;
using Quartz.Impl;
using HRMath.Infraestructure;
using Quartz.Spi;
using Quartz;

namespace HRMath
{
    public class Startup
    {
        private IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment _env)
        {
            Configuration = configuration;
            this._env = _env;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionString"].Replace("%CRP%", _env.ContentRootPath)));

            services.AddIdentity<AppUser, IdentityRole>(opts =>
            {
                opts.Password.RequireDigit = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequiredLength = 6;
                opts.SignIn.RequireConfirmedEmail = false;
                opts.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(opts =>
            {
                opts.LoginPath = "/Account/Login";
            });

            /********** Data Repositories  **********/
            services.AddTransient<IAdRepository, AdRepositoryEF>();
            services.AddTransient<IAuctionRepository, AuctionRepositoryEF>();
            services.AddTransient<IPurchaseRepository, PurchaseRepositoryEF>();
            services.AddTransient<INotificationRepository, NotificationRepositoryEF>();
            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();

            /********** Scheduler Services  **********/
            services.AddSingleton<IJobFactory, CustomQuartzJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<NotificationJob>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(NotificationJob), "Notification Job", "0/10 * * * * ?"));
            services.AddHostedService<CustomQuartzHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            //Very important!!! This two lines MUST be in this order
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => //Setting up the root of the app
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //SeedDatabase(services).Wait();
            //TestComplains(services).Wait();
            //TestSellers(services).Wait();
            //TestAuctions(services).Wait();
            //pop(services);
        }
        public async Task TestAuctions(IServiceProvider serviceProvider)
        {
            var _identityContext = serviceProvider.GetRequiredService<AppDbContext>();

            var ads = _identityContext.Ads.ToList();

            Auction[] auctions = new Auction[]
            {
                new Auction {Product=ads[0].Product, Seller=ads[0].User, Category=ads[0].Category, Description=ads[0].Description, StartPrice=ads[0].Price, StartTimeAuction=DateTime.Now, FinalPrice=ads[0].Price*2, FinalTimeAuction=DateTime.Now},
                new Auction {Product=ads[1].Product, Seller=ads[1].User, Category=ads[1].Category, Description=ads[1].Description, StartPrice=ads[1].Price, StartTimeAuction=DateTime.Now, FinalPrice=ads[1].Price*2, FinalTimeAuction=DateTime.Now},
                new Auction {Product=ads[2].Product, Seller=ads[2].User, Category=ads[2].Category, Description=ads[2].Description, StartPrice=ads[2].Price, StartTimeAuction=DateTime.Now, FinalPrice=ads[2].Price*2, FinalTimeAuction=DateTime.Now},
                new Auction {Product=ads[3].Product, Seller=ads[3].User, Category=ads[3].Category, Description=ads[3].Description, StartPrice=ads[3].Price, StartTimeAuction=DateTime.Now, FinalPrice=ads[3].Price*2, FinalTimeAuction=DateTime.Now}
            };

            _identityContext.Auctions.AddRange(auctions);
            _identityContext.SaveChanges();
        }

        public async Task TestSellers(IServiceProvider serviceProvider)
        {
            var _identityContext = serviceProvider.GetRequiredService<AppDbContext>();

            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            AppUser[] sellers = new AppUser[]
            {
                new AppUser { UserName="Enrique", Email="enrique@example.com"},
                new AppUser { UserName="Maria", Email="maria@example.com"},
                new AppUser { UserName="Henry", Email="henry@example.com"},
                new AppUser { UserName="Kenia", Email="kenia@example.com"},
                new AppUser { UserName="Marcos", Email="marcos@example.com"},
                new AppUser { UserName="Sandra", Email="sandra@example.com"},
                new AppUser { UserName="David", Email="david@example.com"}
            };

            foreach (var s in sellers)
            {
                await _userManager.CreateAsync(s, "password");
                await _userManager.AddToRoleAsync(s, "Seller");
            }

            Ad[] ads = new Ad[]
            {
                new Ad {UnitsAvailable=10,Product = "Ford", Description = " 2011 Ford Escape Limited FWD", Price = 5400, Category = "Vehicles", DateTime = DateTime.Now, User = sellers[0]},
                new Ad {UnitsAvailable=10,Product = "Nissan ", Description = " 2008 Nissan Frontier Se Crew Cab 4x4", Price = 5000, Category = "Vehicles", DateTime = DateTime.Now, User = sellers[0]},
                new Ad {UnitsAvailable=10,Product = "IPhone 7 ", Description = " 16gb storge, Brand new gray cell phone and factory free", Price = 230, Category = "Electronics", DateTime = DateTime.Now, User = sellers[0]},
                new Ad {UnitsAvailable=10,Product = "Razer Gaming Laptop ", Description = "15\" Display, i7-9750H, 16gb DDR4 RAM, NVIDEA GTX1650, 256gb SSD", Price = 1500, Category = "Computers", DateTime = DateTime.Now, User = sellers[0]},
                new Ad {UnitsAvailable=10,Product = "Iphone 11", Description = "Iphone 11:\n64gb storage \n48mpx principal camera \n13mpx secondary camera", Price = 970, Category = "Electronics", DateTime = DateTime.Now, User = sellers[1]},
                new Ad {UnitsAvailable=10,Product = "Apple Watch", Description= " Range: 38 mm", Price= 160, Category = "Electronics", DateTime = DateTime.Now, User = sellers[1]},
                new Ad {UnitsAvailable=10,Product = "Apple Watch", Description= " Range: 42 mm", Price= 230, Category = "Electronics", DateTime = DateTime.Now, User = sellers[1]},
                new Ad {UnitsAvailable=10,Product= " Chubby Lingerie" , Description = " Sexy lingerie for chubby women of all colors", Price = 15,Category= "Clothes", DateTime = DateTime.Now, User = sellers[1]},
                new Ad {UnitsAvailable=10,Product = "TV Samsung 62\"", Description = "62\" 4K Display Smart TV", Price = 550, Category = "Electronics", DateTime = DateTime.Now, User = sellers[2]},
                new Ad {UnitsAvailable=10,Product = "Huawei Mate 20 Lite", Description = "64 GB  storge \n4gb RAM \n4G/LTE" , Price = 250, Category= "Electronics", DateTime = DateTime.Now , User = sellers[2]},
                new Ad {UnitsAvailable=10,Product = "Dell Inspiron 5570 ", Description= "15.6\" Full HD Display, core i7-8550U,  Installed RAM : 8 gb, Internal Memory: 1tb", Price = 500, Category = "Computers", DateTime = DateTime.Now, User= sellers[2]},
                new Ad {UnitsAvailable=10,Product= "Huawei P20 Lite", Description = "64 Gb storge \n4gb RAM \n4G/LTE", Price= 200, Category = "Electronics", DateTime = DateTime.Now, User= sellers[2]},
                new Ad {UnitsAvailable=10,Product= "Mattresses", Description= "Very comfortable personal matresses",Price = 120 , Category= "Furniture",DateTime = DateTime.Now, User = sellers[3]},
                new Ad {UnitsAvailable=10,Product= "Bed with mattress", Description= "Large double bed with a mattress of a size of 153-160 x 203 cm", Price = 250 , Category= "Furniture",DateTime = DateTime.Now, User = sellers[3]},
                new Ad {UnitsAvailable=10,Product= "Living room set ", Description= "Comfortable and modern living room set in black color",Price = 300 , Category= "Furniture",DateTime = DateTime.Now, User = sellers[3]},
                new Ad {UnitsAvailable=10,Product= "Nightstand", Description= "Wooden bedside table with glass stand",Price = 80 , Category= "Furniture", DateTime = DateTime.Now, User = sellers[3]},
                new Ad {UnitsAvailable=10,Product ="Samsung S9", Description ="Resolution: 2,960x1,440 pix, Display: 5.8 inches", Price= 320,  Category = "Electronics", DateTime = DateTime.Now, User = sellers[4]},
                new Ad {UnitsAvailable=10,Product= "Dresses", Description= " short dresses in blue and black color", Price= 20 , Category= "Clothes", DateTime = DateTime.Now, User = sellers[4]},
                new Ad {UnitsAvailable=10,Product= "Dresses", Description= " Long dresses of all the sizes", Price= 25 , Category= "Clothes", DateTime = DateTime.Now, User = sellers[4]},
                new Ad {UnitsAvailable=10,Product= "Jeans", Description= "American Eangle jeans for womens", Price= 25 , Category= "Clothes", DateTime = DateTime.Now, User = sellers[4]},
                new Ad {UnitsAvailable=10,Product = "Xiaomi Mi 9", Description = "5.8\" FHD display, processor Snapdragon 770, 6gb RAM, 128gb storage, 64mpx camera", Price = 430, Category = "Electronics", DateTime = DateTime.Now, User = sellers[5] },
                new Ad {UnitsAvailable=10,Product= "Lingerie" , Description = " Sexy lingerie of all the sizes", Price = 20, Category= "Clothes", DateTime = DateTime.Now, User = sellers[5]},
                new Ad {UnitsAvailable=10,Product= "Shoes", Description= "Nikke shoes for men size 40 ",Price = 70 , Category= "Clothes",DateTime = DateTime.Now, User = sellers[5]},
                new Ad {UnitsAvailable=10,Product= "Shirt", Description= "Blue short-sleeved shirt",Price = 25 , Category= "Clothes",DateTime = DateTime.Now, User = sellers[5]},
                new Ad {UnitsAvailable=10,Product= "Shoes", Description= "Elegant pink square high heel shoes", Price = 30 , Category= "Clothes",DateTime = DateTime.Now, User = sellers[5]},
                new Ad {UnitsAvailable=10,Product= "Lingerie for bride" , Description = " Victoria Secret lingerie for bride in all size", Price = 20, Category= "Clothes", DateTime = DateTime.Now, User = sellers[6]},
                new Ad {UnitsAvailable=10,Product= "Lace Blouse" , Description = "White lance blouse with bare back size s", Price = 10 ,Category= "Clothes", DateTime = DateTime.Now, User = sellers[6]},
                new Ad {UnitsAvailable=10,Product= "Anti-rinkle cream" , Description = "Moisturizing anti-wrinkle cream and protects from the sun", Price = 8 ,Category= "Beauty", DateTime = DateTime.Now, User = sellers[6]},
                new Ad {UnitsAvailable=10,Product= "House in Cuba in the municipality Cerro" , Description = "The house is large with 2 bedrooms, bathroom, kitchen, daining room and living room", Price = 40000 ,Category= "House", DateTime = DateTime.Now, User = sellers[6]},
           };

            _identityContext.Ads.AddRange(ads);
            _identityContext.SaveChanges();

            //Auction[] auctions = new Auction[]
            //{
            //    new Auction {Product=ads[0].Product, Seller=ads[0].User, Category=ads[0].Category, Description=ads[0].Description, StartPrice=ads[0].Price, StartTimeAuction=DateTime.Now, FinalPrice=ads[0].Price*2, FinalTimeAuction=DateTime.Now},
            //    new Auction {Product=ads[1].Product, Seller=ads[1].User, Category=ads[1].Category, Description=ads[1].Description, StartPrice=ads[1].Price, StartTimeAuction=DateTime.Now, FinalPrice=ads[1].Price*2, FinalTimeAuction=DateTime.Now},
            //    new Auction {Product=ads[2].Product, Seller=ads[2].User, Category=ads[2].Category, Description=ads[2].Description, StartPrice=ads[2].Price, StartTimeAuction=DateTime.Now, FinalPrice=ads[2].Price*2, FinalTimeAuction=DateTime.Now},
            //    new Auction {Product=ads[3].Product, Seller=ads[3].User, Category=ads[3].Category, Description=ads[3].Description, StartPrice=ads[3].Price, StartTimeAuction=DateTime.Now, FinalPrice=ads[3].Price*2, FinalTimeAuction=DateTime.Now}
            //};

            //_identityContext.Auctions.AddRange(auctions);
            //_identityContext.SaveChanges();
        }

        public async Task TestComplains(IServiceProvider serviceProvider)
        {
            var _identityContext = serviceProvider.GetRequiredService<AppDbContext>();
            bool created = _identityContext.Database.EnsureCreated();

            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            var joe = new AppUser { UserName = "Joe", Email = "joe@example.com" };
            await _userManager.CreateAsync(joe, "password");
            await _userManager.AddToRoleAsync(joe, "Seller");

            var max = new AppUser { UserName = "Max", Email = "max@example.com" };
            await _userManager.CreateAsync(max, "password");
            await _userManager.AddToRoleAsync(max, "Buyer");

            var ad = new Ad { Product = "Una papa", Description = "Muy Rica", Price = 45, Category = "Food", DateTime = DateTime.Now, User = joe };
            _identityContext.Ads.Add(ad);
            _identityContext.SaveChanges();
        }

        public async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            var _identityContext = serviceProvider.GetRequiredService<AppDbContext>();
            bool created = _identityContext.Database.EnsureCreated();

            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            AppUser[] admins = new AppUser[] { new AppUser {UserName = "Victor", Email = "victor@example.com"},
                                               new AppUser {UserName = "Karla", Email = "karla@example.com"},
                                               new AppUser {UserName = "Amanda", Email = "amanda@example.com"},
                                               new AppUser {UserName = "Thalia", Email = "thalia@example.com"}};

            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("Buyer"));
            await _roleManager.CreateAsync(new IdentityRole("Seller"));

            foreach (var a in admins)
            {
                await _userManager.CreateAsync(a, "password");
                await _userManager.AddToRoleAsync(a, "Admin");
                var ad = new Ad { Product = "Una papa", Description = "Muy Rica", Price = 45, Category = "Food", DateTime = DateTime.Now, User = a };
            }
        }
    }
}
