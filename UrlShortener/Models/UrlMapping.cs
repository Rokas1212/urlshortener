using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Models
{
    public class UrlMapping
    {
        public int Id { get; set; }

        [Required]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? OriginalUrl { get; set; }

        public string? ShortenedKey { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpiresOn { get; set; }


        [ForeignKey("ShortenedKey")]
        public virtual QrCode? QrCode { get; set; }
    }
}
