using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Models
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<UrlMapping> Urls { get; set; } = new List<UrlMapping>();
    }
}
