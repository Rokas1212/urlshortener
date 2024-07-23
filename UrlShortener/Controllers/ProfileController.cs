using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    public class ProfileController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public ProfileController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var userId = _userManager.GetUserId(User);

            var user = await _userManager.Users
                                         .Include(u => u.Urls)
                                         .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return BadRequest();

            return View(user);
        }

        [HttpGet]
        public async Task<string> GetQrCode(string shortenedKey)
        {
            var qrCode = await _context.QrCodes.FirstOrDefaultAsync(q => q.ShortenedKey == shortenedKey);

            if (qrCode == null)
                return string.Empty;

            return qrCode.Base64QrCode!;
        }

    }
}
