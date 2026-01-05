using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
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
    public class MoviesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IWebHostEnvironment env) : Controller
    {
        private readonly ApplicationDbContext db = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IWebHostEnvironment _env = env;

        // [Authorize(Roles = "User,Editor,Admin")] // Allow everyone to view
        public IActionResult Index()
        {
            IQueryable<Movie> movies = db.Movies
                .Include(m => m.Genre)
                .Include(m => m.User)
                .Include(m => m.ActorMovies).ThenInclude(am => am.Actor)
                .Include(m => m.MovieDiaries)
                .Include(m => m.Reviews)
                .AsQueryable();

            if(TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            
            var search = "";
            if(Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                
                List<int> articleIds = db.Movies.Where(
                    at => at.Title.Contains(search) ||
                          at.Director.Contains(search) ||
                          at.Description.Contains(search)
                ).Select(at => at.Id).ToList();
                
                movies = movies.Where(m => articleIds.Contains(m.Id));
            }
            
            ViewBag.SearchString = search;

            int _perPage = 3;
            int totalItems = movies.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedMovies = movies.Skip(offset).Take(_perPage).ToList();
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Movies = paginatedMovies;
            ViewBag.Articles = paginatedMovies;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Movies/Index?search=" + search + "&page=";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Movies/Index?page=";
            }

            return View();
        }

        // [Authorize(Roles = "User,Editor,Admin")] // Allow everyone to view

        public IActionResult Show(int id)
        {
            var movie = db.Movies
                .Include(m => m.Genre)
                .Include(m => m.User)
                .Include(m => m.ActorMovies).ThenInclude(am => am.Actor)
                .Include(m => m.MovieDiaries)
                .Include(m => m.Reviews).ThenInclude(r => r.User)
                .Include(m => m.Reviews).ThenInclude(r => r.ReviewVotes)
                .FirstOrDefault(m => m.Id == id);

            if (movie is null)
            {
                return NotFound();
            }

            if(TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();
            return View(movie);
        }
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;
            
            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;
            }
            // Allow basic users to see buttons for their own content (handled in view usually, but setting flag here)
            // Actually, existing logic was restrictive. Let's rely on View logic examining current user
            // but here we just pass data.
            ViewBag.CurrentUser = _userManager.GetUserId(User);

            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }

        [Authorize(Roles = "Editor,Admin")]
        public IActionResult New()
        {
            ViewBag.Genres = GetAllGenres();

            return View(new Movie
            {
                ReleaseDate = DateTime.Today
            });
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> New(Movie movie, IFormFile? Image)
        {
            var sanitizer = new HtmlSanitizer();

            movie.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    var storagePath = Path.Combine(_env.WebRootPath, "images", "movies");
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
                    movie.ImageUrl = "/images/movies/" + fileName;
                }

                movie.Description = sanitizer.Sanitize(movie.Description);
                db.Movies.Add(movie);
                await db.SaveChangesAsync();
                TempData["message"] = "Movie added successfully!";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }

            ViewBag.Genres = GetAllGenres();
            return View(movie);
        }

        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Edit(int id)
        {
            var movie = db.Movies
                .Include(m => m.Genre)
                .FirstOrDefault(m => m.Id == id);

            if (movie is null)
            {
                return NotFound();
            }

            ViewBag.Genres = GetAllGenres();

            if ((movie.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
            {
                return View(movie);
            }

            TempData["message"] = "Nu aveți dreptul să modificați un film care nu vă aparține.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Delete(int id)
        {
            var movie = db.Movies.Find(id);

            if (movie is null)
            {
                return NotFound();
            }
            else
            {
                if(movie.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    db.Movies.Remove(movie);
                    db.SaveChanges();

                    TempData["message"] = "Filmul a fost șters.";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa stergeti un film care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
        }
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> Edit(int id, Movie requestMovie, IFormFile? Image)
        {
            var sanitizer = new HtmlSanitizer();
            var movie = await db.Movies.FindAsync(id);

            if (movie is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Genres = GetAllGenres();
                return View(requestMovie);
            }

            if ((movie.UserId == _userManager.GetUserId(User)) || User.IsInRole("Admin"))
            {
                movie.Title = requestMovie.Title;
                movie.Director = requestMovie.Director;
                movie.Description = sanitizer.Sanitize(requestMovie.Description);
                movie.Score = requestMovie.Score;
                movie.ReleaseDate = requestMovie.ReleaseDate;
                movie.GenreId = requestMovie.GenreId;
                
                if (Image != null && Image.Length > 0)
                {
                    var storagePath = Path.Combine(_env.WebRootPath, "images", "movies");
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
                    movie.ImageUrl = "/images/movies/" + fileName;
                }
                else 
                {
                    // Keep existing if not null, or logic if users can paste URL ? 
                    // Current request is for local files, but we might want to fallback to the existing URL if user didn't upload
                    // But here requestMovie.ImageUrl might come from hidden field? No hidden field in View.
                    // If Image is null, we just don't update ImageUrl, so it keeps old value.
                    // EXCEPT if we want to allow clearing it? For now, let's just update if file provided.
                }

                await db.SaveChangesAsync();

                TempData["message"] = "Filmul a fost actualizat.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            TempData["message"] = "Nu aveți dreptul să modificați un film care nu vă aparține.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        private IEnumerable<SelectListItem> GetAllGenres()
        {
            return db.Genres
                .OrderBy(g => g.GenreName)
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.GenreName
                })
                .ToList();
        }


    }

}