using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;

namespace SocialFilmPlatform.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Show(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Diaries)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                // If no userId provided, try to show current user profile
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null && string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Show", new { userId = currentUser.Id });
                }
                return NotFound();
            }

            var viewer = await _userManager.GetUserAsync(User);
            bool isOwner = viewer != null && viewer.Id == user.Id;

            // Filter diaries: Show all if owner, else show only public
            var visibleDiaries = isOwner 
                ? user.Diaries 
                : user.Diaries.Where(d => d.IsPublic).ToList();

            ViewBag.IsOwner = isOwner;
            ViewBag.Diaries = visibleDiaries;

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ApplicationUser model, IFormFile? ProfileImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Description = model.Description;

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var storagePath = Path.Combine(_env.WebRootPath, "images", "profiles");
                if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                var filePath = Path.Combine(storagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                user.ProfilePictureUrl = "/images/profiles/" + fileName;
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction("Show", new { userId = user.Id });
        }
    }
}
