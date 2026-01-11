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
