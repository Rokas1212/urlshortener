using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
        }

        public DbSet<UrlMapping> Urls { get; set; }
        public DbSet<QrCode> QrCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlMapping>()
                .HasOne(u => u.QrCode)
                .WithOne(q => q.UrlMapping)
                .HasForeignKey<UrlMapping>(u => u.ShortenedKey);

            base.OnModelCreating(modelBuilder);
        }
    }
}
