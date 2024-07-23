using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;

namespace UrlShortener.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class QrCodeController : Controller
    {
        private readonly AppDbContext _context;

        public QrCodeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetQrCode")]
        public async Task<string> GetQrCode(string shortenedKey)
        {
            var qrCode = await _context.QrCodes.FirstOrDefaultAsync(q => q.ShortenedKey == shortenedKey);

            if (qrCode == null)
                return string.Empty;

            return qrCode.Base64QrCode!;
        }
    }
}
