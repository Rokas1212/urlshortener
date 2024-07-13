using UrlShortener.Data;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Services;


namespace UrlShortener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



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

            app.UseAuthorization();

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

            app.Run();
        }
    }
}
