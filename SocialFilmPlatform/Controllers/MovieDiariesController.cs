using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;

namespace SocialFilmPlatform.Controllers
{
    public class MovieDiariesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly ApplicationDbContext db = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Index()
        {
            var movieDiaries = db.MovieDiaries.Include(md => md.Movie).Include(md => md.Diary);
            
            ViewBag.MovieDiaries = movieDiaries;
            
            return View();
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New()
        {
            ViewBag.Movies = new SelectList(db.Movies, "Id", "Title");
            ViewBag.Diaries = new SelectList(db.Diaries, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New(MovieDiary movieDiary)
        {
            var diary = db.Diaries.Find(movieDiary.DiaryId);
            var currentUserId = _userManager.GetUserId(User);

            // Validation: Does diary exist? Is it mine?
            if (diary == null || (diary.UserId != currentUserId && !User.IsInRole("Admin")))
            {
                TempData["message"] = "You can only add movies to your own diaries!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Diaries");
            }

            // Check duplicate
            var existing = db.MovieDiaries.FirstOrDefault(md => md.DiaryId == movieDiary.DiaryId && md.MovieId == movieDiary.MovieId);
            if (existing != null)
            {
                TempData["message"] = "Movie already in diary!";
                TempData["messageType"] = "alert-warning";
                 return RedirectToAction("Show", "Diaries", new { id = diary.Id });
            }

            if (ModelState.IsValid)
            {
               movieDiary.DiaryTime = DateTime.Now; 
                
                db.MovieDiaries.Add(movieDiary);
                db.SaveChanges();
                TempData["message"] = "Movie added to diary!";
                TempData["messageType"] = "success";
                return RedirectToAction("Show", "Diaries", new { id = diary.Id });
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
