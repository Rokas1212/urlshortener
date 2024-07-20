using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.Services;
using UrlShortener.ViewModels;

namespace UrlShortener.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class UrlShortenerController : Controller
    {

        private readonly AppDbContext _context;
        private readonly QrCodeService _qrCodeService;
        private readonly UserManager<AppUser> _userManager;

        public UrlShortenerController(AppDbContext context, QrCodeService qrCodeService, UserManager<AppUser> userManager)
        {
            _context = context;
            _qrCodeService = qrCodeService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index(UrlMapping? model)
        {
            if (model?.ShortenedKey != null)
            {
                var qrCode = _context.QrCodes.FirstOrDefault(q => q.ShortenedKey == model.ShortenedKey);
                if (qrCode != null)
                {
                    ViewBag.QrCodeImage = $"data:image/png;base64,{qrCode.Base64QrCode}";
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult ShortenUrl(UrlMapping model)
        {
            if (ModelState.IsValid)
            {
                string shortenedKey = GenerateShortenedKey();

                model.ShortenedKey = shortenedKey;
                model.CreatedOn = DateTime.Now.ToUniversalTime();
                model.ExpiresOn = model.ExpiresOn.ToUniversalTime();
                model.UserId = GetCurrentUserId();
                    
                GenerateQrCode(model);

                _context.Urls.Add(model);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", model);
        }


        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public async Task<IActionResult> EditUrl([FromBody] EditUrlViewModel model)
        {
            if (ModelState.IsValid)
            {
                var urlMapping = await _context.Urls.FindAsync(model.Id);
                if (urlMapping == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (urlMapping.UserId != GetCurrentUserId() && (await _userManager.IsInRoleAsync(currentUser!, "Admin")))
                {
                    return Forbid();
                }

                urlMapping.OriginalUrl = model.NewUrl;
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
        }

        private void GenerateQrCode(UrlMapping urlModel)
        {
            if (ModelState.IsValid)
            {
                var qrModel = new QrCode
                {
                    ShortenedKey = urlModel.ShortenedKey,
                    Base64QrCode = _qrCodeService.GenerateQrCode($"https://localhost:7070/r/{urlModel.ShortenedKey}")
                };

                _context.QrCodes.Add(qrModel);
                _context.SaveChanges();
            }

        }
        private string GenerateShortenedKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var key = new string(Enumerable.Repeat(chars, 7)
            .Select(s => s[random.Next(s.Length)]).ToArray());

            return key;
        }

        [AllowAnonymous]
        [HttpGet("/r/{key}")]
        public IActionResult RedirectToOriginal(string key)
        {
            var urlMapping = _context.Urls.FirstOrDefault(x => x.ShortenedKey == key);

            if (urlMapping != null && urlMapping.ExpiresOn > DateTime.UtcNow)
            {
                return Redirect(urlMapping.OriginalUrl!);
            }
            return NotFound();
        }


        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
