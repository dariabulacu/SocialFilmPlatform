using Microsoft.AspNetCore.Authorization;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public ProfilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var usersQuery = _context.Users.AsQueryable();


            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId != null)
            {
                usersQuery = usersQuery.Where(u => u.Id != currentUserId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                usersQuery = usersQuery.Where(u => 
                    u.UserName.Contains(search) || 
                    (u.FirstName != null && u.FirstName.Contains(search)) || 
                    (u.LastName != null && u.LastName.Contains(search)));
                
                ViewBag.SearchString = search;
            }
            

            var users = await usersQuery.Take(50).ToListAsync();
            
            return View(users);
        }

        public async Task<IActionResult> Show(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Diaries)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null && string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Show", new { userId = currentUser.Id });
                }
                return NotFound();
            }

            var viewer = await _userManager.GetUserAsync(User);
            bool isOwner = viewer != null && viewer.Id == user.Id;


            var visibleDiaries = isOwner 
                ? user.Diaries 
                : user.Diaries.Where(d => d.IsPublic).ToList();

            ViewBag.IsOwner = isOwner;
            ViewBag.Diaries = visibleDiaries;


            if (User.IsInRole("Admin"))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                ViewBag.UserRoles = userRoles;
                ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            }

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["message"] = $"Role changed to {newRole} for user {user.UserName}";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Show", new { userId = userId });
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
