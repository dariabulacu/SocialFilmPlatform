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
            if (ModelState.IsValid)
            {
               // Typically check simply if it exists, or allow duplicates if it's a journal?
               // Assuming unique entry per diary per movie isn't strictly enforced by DB constraint shown in snippets, but logical.
               // However, a Diary might allow re-watching? 
               // For now, simple Add.
               movieDiary.DiaryTime = DateTime.Now; // Default timestamp
                
                db.MovieDiaries.Add(movieDiary);
                db.SaveChanges();
                TempData["message"] = "Movie added to diary!";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }
            ViewBag.Movies = new SelectList(db.Movies, "Id", "Title");
            ViewBag.Diaries = new SelectList(db.Diaries, "Id", "Name");
            return View(movieDiary);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Delete(int id)
        {
             var movieDiary = db.MovieDiaries.Find(id);
             if (movieDiary != null)
             {
                 db.MovieDiaries.Remove(movieDiary);
                 db.SaveChanges();
                 TempData["message"] = "Entry removed from diary!";
                 TempData["messageType"] = "success";
             }
             return RedirectToAction("Index");
        }
    }
}
