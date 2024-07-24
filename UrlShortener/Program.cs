using UrlShortener.Data;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Services;
using Microsoft.AspNetCore.Identity;
using UrlShortener.Models;
using UrlShortener.Services.ServiceInterfaces;


namespace UrlShortener
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var baseConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            var dbPassword = builder.Configuration["DbPassword"];

            var connectionString = $"{baseConnectionString};Password={dbPassword}";

            Console.WriteLine(connectionString);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services
                .AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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

            app.UseAuthentication();
            app.UseAuthorization();

            // Default Route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Route for URLShortenerController
            app.MapControllerRoute(
                name: "urlshortener",
                pattern: "{controller=UrlShortener}/{action=Index}/{id?}");

            // Route for Auth
            app.MapControllerRoute(
                name: "auth",
                pattern: "{controller=Auth}/{action=Index}/{id?}"
                );

            // Route for User Profile
            app.MapControllerRoute(
                name: "profile",
                pattern: "{controller=Profile}/{action=Index}/{id?}");


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
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                var admin = "admin@admin.com";
                var user = "user@user.com";
                var password = "Test123!";

                //Seed admin account
                if (await userManager.FindByEmailAsync(admin) == null)
                {
                    var newAdmin = new AppUser();
                    newAdmin.UserName = admin;
                    newAdmin.Email = admin;
                    newAdmin.EmailConfirmed = true;

                    await userManager.CreateAsync(newAdmin, password);

                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }

                //Seed user account
                if (await userManager.FindByEmailAsync(user) == null)
                {
                    var newUser = new AppUser();
                    newUser.UserName = user;
                    newUser.Email = user;
                    newUser.EmailConfirmed = true;

                    await userManager.CreateAsync(newUser, password);

                    await userManager.AddToRoleAsync(newUser, "User");
                }
            }

            app.Run();
        }
    }
}
