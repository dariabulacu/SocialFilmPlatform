using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;

namespace SocialFilmPlatform.Controllers
{
    public class ActorMoviesController(
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
            var actorMovies = db.Actors.SelectMany(a => a.ActorMovies).Include(am => am.Actor).Include(am => am.Movie);
            // Or access db.Set<ActorMovie>() if exposed, but DbSet<ActorMovie> is not explicitly in DbContext code I saw?
            // Wait, logic check: Step 27 showed `public DbSet<Actor> Actors` but checking `ActorMovie`:
            // It has `public DbSet<ActorMovie> ActorMovies`? Let me re-verify Step 27 output.
            // Yes, line 41: `modelBuilder.Entity<ActorMovie>()` but line 18 is `DbSet<Actor>`. 
            // Wait, strict check on DbSet properties.
            // Line 14-20: Movies, Genres, Reviews, Diaries, Actors, MovieDiaries, ApplicationUsers.
            // `ActorMovies` IS NOT a DbSet. So I cannot access `db.ActorMovies` directly unless I cast `Set<ActorMovie>()`.
            // I should use `db.Set<ActorMovie>()`.
            
            ViewBag.ActorMovies = actorMovies;
            
            return View();
        }

        [Authorize(Roles = "Editor,Admin")]
        public IActionResult New()
        {
            ViewBag.Actors = new SelectList(db.Actors, "Id", "Name");
            ViewBag.Movies = new SelectList(db.Movies, "Id", "Title");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult New(ActorMovie actorMovie)
        {
            if (ModelState.IsValid)
            {
                // Accessing db.Set<ActorMovie> since it's not a property
                var existing = db.Set<ActorMovie>().FirstOrDefault(am => am.ActorId == actorMovie.ActorId && am.MovieId == actorMovie.MovieId);
                if (existing == null) {
                    db.Set<ActorMovie>().Add(actorMovie);
                    db.SaveChanges();
                    TempData["message"] = "Actor assigned to movie successfully!";
                    TempData["messageType"] = "success";
                    return RedirectToAction("Index"); // Or back to Movie/Actor show page
                } 
                 else 
                {
                    TempData["message"] = "Relationship already exists!";
                    TempData["messageType"] = "alert-warning";
                }
            }
            ViewBag.Actors = new SelectList(db.Actors, "Id", "Name");
            ViewBag.Movies = new SelectList(db.Movies, "Id", "Title");
            return View(actorMovie);
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Delete(int id)
        {
             var actorMovie = db.Set<ActorMovie>().Find(id);
             if (actorMovie != null)
             {
                 db.Set<ActorMovie>().Remove(actorMovie);
                 db.SaveChanges();
                 TempData["message"] = "Relationship removed!";
                 TempData["messageType"] = "success";
             }
             return RedirectToAction("Index");
        }
    }
}
