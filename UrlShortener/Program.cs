using UrlShortener.Data;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Services;
using Microsoft.AspNetCore.Identity;


namespace UrlShortener
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services
                .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();



            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddRazorPages();

            // Register QrCodeService
            builder.Services.AddScoped<QrCodeService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => 
            {
                endpoints.MapRazorPages();
            });

            // Default Route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Route for URLShortenerController
            app.MapControllerRoute(
                name: "urlshortener",
                pattern:"{controller=UrlShortener}/{action=Index}/{id?}");

            // Route for handling shortened URLs
            app.MapControllerRoute(
                name: "redirect",
                pattern: "/r/{key}",
                defaults: new { controller = "URLShortener", action = "RedirectToOriginal" });



            //Seeding roles
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                
                var roles = new[] { "Admin", "User" };

                foreach (var role in roles)
                {

                    if(!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }

                }
            }

            //Seeding users

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var admin = "admin@admin.com";
                var user = "user@user.com";
                var password = "Test123!";

                //Seed admin account
                if (await userManager.FindByEmailAsync(admin) == null)
                {
                    var newAdmin = new IdentityUser();
                    newAdmin.UserName = admin;
                    newAdmin.Email = admin;

                    await userManager.CreateAsync(newAdmin, password);

                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }

                //Seed user account
                if (await userManager.FindByEmailAsync(user) == null)
                {
                    var newUser = new IdentityUser();
                    newUser.UserName = user;
                    newUser.Email = user;

                    await userManager.CreateAsync(newUser, password);

                    await userManager.AddToRoleAsync(newUser, "User");
                }
            }

            app.Run();
        }
    }
}
