using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{
    public class QrCode
    {
        [Key]
        public string? ShortenedKey { get; set; }

        [Required]
        public string? Base64QrCode { get; set; }

        public virtual UrlMapping UrlMapping { get; set; }
    }
}
