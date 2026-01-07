using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;
using SocialFilmPlatform.Services;

namespace SocialFilmPlatform.Controllers
{
    public class MovieDiariesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IAiService aiService) : Controller
    {
        private readonly ApplicationDbContext db = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IAiService _aiService = aiService;

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Index()
        {
            var movieDiaries = db.MovieDiaries.Include(md => md.Movie).Include(md => md.Diary);
            
            ViewBag.MovieDiaries = movieDiaries;
            
            return View();
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New(int? diaryId)
        {
            var currentUserId = _userManager.GetUserId(User);
            ViewBag.Movies = new SelectList(db.Movies.OrderBy(m => m.Title), "Id", "Title");
            
            var userDiaries = db.Diaries.Where(d => d.UserId == currentUserId).OrderBy(d => d.CreatedAt);
            ViewBag.Diaries = new SelectList(userDiaries, "Id", "Name", diaryId);
            
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetMyDiaries()
        {
            var currentUserId = _userManager.GetUserId(User);
            var diaries = db.Diaries
                .Where(d => d.UserId == currentUserId)
                .Select(d => new { d.Id, d.Name })
                .ToList();
            return Json(diaries);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public async Task<IActionResult> New(MovieDiary movieDiary, string? returnUrl = null, List<string>? SelectedTags = null, List<string>? SelectedCategories = null)
        {
            var diary = db.Diaries.Find(movieDiary.DiaryId);
            var currentUserId = _userManager.GetUserId(User);

            // Validation: Does diary exist? Is it mine?
            if (diary == null || (diary.UserId != currentUserId && !User.IsInRole("Admin")))
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    TempData["message"] = "You can only add movies to your own lists!";
                    TempData["messageType"] = "alert-danger";
                    return LocalRedirect(returnUrl);
                }
                TempData["message"] = "You can only add movies to your own diaries!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Diaries");
            }

            // Check duplicate
            var existing = db.MovieDiaries.FirstOrDefault(md => md.DiaryId == movieDiary.DiaryId && md.MovieId == movieDiary.MovieId);
            if (existing != null)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                     TempData["message"] = "Movie is already in this list.";
                     TempData["messageType"] = "alert-warning";
                     return LocalRedirect(returnUrl);
                }
                TempData["message"] = "Movie already in diary!";
                TempData["messageType"] = "alert-warning";
                 return RedirectToAction("Show", "Diaries", new { id = diary.Id });
            }

            // Manually populate fields that might be causing validation errors
            movieDiary.DiaryTime = DateTime.Now;
            if (string.IsNullOrEmpty(movieDiary.Name)) movieDiary.Name = "Entry"; // dummy value to satisfy required if any

            // Clear validation errors for fields we just set or don't care about
            ModelState.Remove(nameof(movieDiary.DiaryTime));
            ModelState.Remove(nameof(movieDiary.Name));
            ModelState.Remove(nameof(movieDiary.Movie));
            ModelState.Remove(nameof(movieDiary.Diary));

            if (ModelState.IsValid)
            {
                db.MovieDiaries.Add(movieDiary);
                db.SaveChanges();
                
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    TempData["message"] = "Movie added to list!";
                    TempData["messageType"] = "success";
                    return LocalRedirect(returnUrl);
                }
                
                TempData["message"] = "Movie added to diary!";
                TempData["messageType"] = "success";
                return RedirectToAction("Show", "Diaries", new { id = diary.Id });
            }
            
            // If invalid and returnUrl exists, we should probably still go back
            if (!string.IsNullOrEmpty(returnUrl))
            {
                 // Debug: what failed?
                 var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                 TempData["message"] = "Failed to add movie. " + errors; 
                 TempData["messageType"] = "alert-danger";
                 return LocalRedirect(returnUrl);
            }
            
            ViewBag.Movies = new SelectList(db.Movies, "Id", "Title");
            ViewBag.Diaries = new SelectList(db.Diaries.Where(d => d.UserId == currentUserId), "Id", "Name");
            return View(movieDiary);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Delete(int id)
        {
             var movieDiary = db.MovieDiaries.Include(md => md.Diary).FirstOrDefault(md => md.Id == id);
             if (movieDiary != null)
             {
                 var currentUserId = _userManager.GetUserId(User);
                 if (movieDiary.Diary.UserId != currentUserId && !User.IsInRole("Admin"))
                 {
                    TempData["message"] = "Unauthorized action!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                 }

                 db.MovieDiaries.Remove(movieDiary);
                 db.SaveChanges();
                 TempData["message"] = "Entry removed from diary!";
                 TempData["messageType"] = "success";
                 return RedirectToAction("Show", "Diaries", new { id = movieDiary.DiaryId });
             }
             return RedirectToAction("Index");
        }
    }
}
