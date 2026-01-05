using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;
using Ganss.Xss;

namespace SocialFilmPlatform.Controllers
{
    public class ActorsController(
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
            var actors = db.Actors.Include(a => a.ActorMovies).ThenInclude(am => am.Movie);
            
            ViewBag.Actors = actors;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show(int id)
        {
            var actor = db.Actors
                .Include(a => a.ActorMovies).ThenInclude(am => am.Movie)
                .FirstOrDefault(a => a.Id == id);

            if (actor is null)
            {
                return NotFound();
            }
            SetAccessRights();
            return View(actor);
        }

        [Authorize(Roles = "Editor,Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult New(Actor actor)
        {
            if (ModelState.IsValid)
            {
                db.Actors.Add(actor);
                db.SaveChanges();
                TempData["message"] = "Actor added successfully!";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }
            return View(actor);
        }

        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Edit(int id)
        {
            var actor = db.Actors.Find(id);
            if (actor is null)
            {
                return NotFound();
            }
            return View(actor);
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Edit(int id, Actor requestActor)
        {
            var actor = db.Actors.Find(id);
            if (actor is null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                actor.Name = requestActor.Name;
                actor.Description = requestActor.Description;
                db.SaveChanges();
                TempData["message"] = "Actor updated successfully!";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }
            return View(requestActor);
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Delete(int id)
        {
            var actor = db.Actors.Find(id);
            if (actor is null)
            {
                return NotFound();
            }
            
            db.Actors.Remove(actor);
            db.SaveChanges();
            TempData["message"] = "Actor deleted successfully!";
            TempData["messageType"] = "success";
            return RedirectToAction("Index");
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;
            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }
    }
}
