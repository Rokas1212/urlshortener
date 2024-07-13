using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    public class UrlShortenerController : Controller
    {

        private readonly AppDbContext _context;
        private readonly QrCodeService _qrCodeService;

        public UrlShortenerController(AppDbContext context, QrCodeService qrCodeService) 
        {
            _context = context;
            _qrCodeService = qrCodeService;
        }
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
        public IActionResult ShortenUrl(UrlMapping model)
        {
            if (ModelState.IsValid)
            {
                string shortenedKey = GenerateShortenedKey();

                model.ShortenedKey = shortenedKey;
                model.CreatedOn = DateTime.Now.ToUniversalTime();
                model.ExpiresOn = model.ExpiresOn.ToUniversalTime();

                GenerateQrCode(model);

                _context.Urls.Add(model);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", model);
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

    }
}
