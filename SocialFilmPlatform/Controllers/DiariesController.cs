using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;

namespace SocialFilmPlatform.Controllers
{
    public class DiariesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly ApplicationDbContext db = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        // [Authorize(Roles = "User,Editor,Admin")] // Public access
        public IActionResult Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            // Public diaries OR my diaries
            var diaries = db.Diaries
                            .Where(d => d.IsPublic || d.UserId == currentUserId)
                            .Include(d => d.User)
                            .Include(d => d.MovieDiaries).ThenInclude(md => md.Movie);
            
            ViewBag.Diaries = diaries;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        // [Authorize(Roles = "User,Editor,Admin")] // Public access (with internal checks)
        public IActionResult Show(int id)
        {
            var diary = db.Diaries
                .Include(d => d.User)
                .Include(d => d.MovieDiaries).ThenInclude(md => md.Movie)
                .FirstOrDefault(d => d.Id == id);

            if (diary is null)
            {
                return NotFound();
            }

            // Privacy check: if private and not owner, deny access
            var currentUserId = _userManager.GetUserId(User);
            // If user is not logged in, currentUserId is null. Private diaries check works: (false && false && !User.IsInRole) -> Forbid
            // Public diaries: IsPublic = true -> skipped if body
            if (!diary.IsPublic && diary.UserId != currentUserId && !User.IsInRole("Admin")) 
            {
                return Forbid();
            }

            SetAccessRights();
            return View(diary);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New(Diary diary)
        {
            diary.UserId = _userManager.GetUserId(User);
            
            if (ModelState.IsValid)
            {
                db.Diaries.Add(diary);
                db.SaveChanges();
                TempData["message"] = "Diary created successfully!";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }
            return View(diary);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            var diary = db.Diaries.Find(id);
            if (diary is null)
            {
                return NotFound();
            }
            
            var currentUserId = _userManager.GetUserId(User);
            if (diary.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                 TempData["message"] = "You cannot edit this diary!";
                 TempData["messageType"] = "alert-danger";
                 return RedirectToAction("Index");
            }

            return View(diary);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Diary requestDiary)
        {
            var diary = db.Diaries.Find(id);
            if (diary is null)
            {
                return NotFound();
            }
            
            var currentUserId = _userManager.GetUserId(User);
            if (diary.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                 return Forbid();
            }

            if (ModelState.IsValid)
            {
                diary.Name = requestDiary.Name;
                diary.IsPublic = requestDiary.IsPublic;
                db.SaveChanges();
                TempData["message"] = "Diary updated successfully!";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }
            return View(requestDiary);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Delete(int id)
        {
            var diary = db.Diaries.Find(id);
            if (diary is null)
            {
                return NotFound();
            }
            
            var currentUserId = _userManager.GetUserId(User);
            if (diary.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                 TempData["message"] = "You cannot delete this diary!";
                 TempData["messageType"] = "alert-danger";
                 return RedirectToAction("Index");
            }
            
            db.Diaries.Remove(diary);
            db.SaveChanges();
            TempData["message"] = "Diary deleted successfully!";
            TempData["messageType"] = "success";
            return RedirectToAction("Index");
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = true; // Assuming owners can edit, implementing generic for now
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }
    }
}
