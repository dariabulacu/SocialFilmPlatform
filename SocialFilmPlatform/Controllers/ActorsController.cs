using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SocialFilmPlatform.Controllers
{
    public class ActorsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IWebHostEnvironment env) : Controller
    {
        private readonly ApplicationDbContext db = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IWebHostEnvironment _env = env;

        // [Authorize(Roles = "User,Editor,Admin")] // Public access
        public IActionResult Index()
        {
            var search = "";
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
            }

            var actorsQuery = db.Actors
                .Include(a => a.ActorMovies).ThenInclude(am => am.Movie)
                .OrderBy(a => a.Name)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                List<int> actorIds = db.Actors.Where(
                    a => a.Name.Contains(search) ||
                         a.Description.Contains(search)
                ).Select(a => a.Id).ToList();

                actorsQuery = actorsQuery.Where(a => actorIds.Contains(a.Id));
            }

            ViewBag.SearchString = search;

            int _perPage = 3;
            int totalItems = actorsQuery.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedActors = actorsQuery.Skip(offset).Take(_perPage).ToList();
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Actors = paginatedActors;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Actors/Index?search=" + search + "&page=";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Actors/Index?page=";
            }

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            // Set access rights for buttons in view
            SetAccessRights();
            
            return View();
        }

        // [Authorize(Roles = "User,Editor,Admin")] // Public access
        public IActionResult Show(int id)
        {
            var actor = db.Actors
                .Include(a => a.ActorMovies).ThenInclude(am => am.Movie)
                .FirstOrDefault(a => a.Id == id);

            if (actor is null)
            {
                return NotFound();
            }

            // Get movies not already associated with this actor
            var existingMovieIds = actor.ActorMovies.Select(am => am.MovieId).ToList();
            var availableMovies = db.Movies
                .Where(m => !existingMovieIds.Contains(m.Id))
                .OrderBy(m => m.Title)
                .Select(m => new { m.Id, m.Title }) // Projection for efficiency
                .ToList();

            ViewBag.AllMovies = new SelectList(availableMovies, "Id", "Title");

            SetAccessRights();
            return View(actor);
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult AddMovie([FromForm] int ActorId, [FromForm] int MovieId)
        {
            if (ActorId == 0 || MovieId == 0)
            {
                TempData["message"] = "Invalid selection.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", new { id = ActorId });
            }

            var existing = db.Set<ActorMovie>()
                .FirstOrDefault(am => am.ActorId == ActorId && am.MovieId == MovieId);

            if (existing == null)
            {
                var actorMovie = new ActorMovie
                {
                    ActorId = ActorId,
                    MovieId = MovieId,
                    Name = "-"
                };
                db.Set<ActorMovie>().Add(actorMovie);
                db.SaveChanges();
                TempData["message"] = "Film added to filmography!";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Actor is already associated with this film.";
                TempData["messageType"] = "alert-warning";
            }

            return RedirectToAction("Show", new { id = ActorId });
        }

        [Authorize(Roles = "Editor,Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> New(Actor actor, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    var storagePath = Path.Combine(_env.WebRootPath, "images", "actors");
                    if (!Directory.Exists(storagePath))
                    {
                        Directory.CreateDirectory(storagePath);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    var filePath = Path.Combine(storagePath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }
                    actor.PhotoUrl = "/images/actors/" + fileName;
                }

                db.Actors.Add(actor);
                await db.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(int id, Actor requestActor, IFormFile? Image)
        {
            var actor = await db.Actors.FindAsync(id);
            if (actor is null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                actor.Name = requestActor.Name;
                actor.Description = requestActor.Description;
                
                if (Image != null && Image.Length > 0)
                {
                    var storagePath = Path.Combine(_env.WebRootPath, "images", "actors");
                    if (!Directory.Exists(storagePath))
                    {
                        Directory.CreateDirectory(storagePath);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    var filePath = Path.Combine(storagePath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }
                    actor.PhotoUrl = "/images/actors/" + fileName;
                }

                await db.SaveChangesAsync();
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
