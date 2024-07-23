using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    public class ProfileController : Controller
    {

        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
    }
}
