using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;
using Ganss.Xss;


namespace SocialFilmPlatform.Controllers
{
    public class MoviesController(
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
            var movies = db.Movies
                .Include(m => m.Genre)
                .Include(m => m.User)
                .Include(m => m.ActorMovies).ThenInclude(am => am.Actor)
                .Include(m => m.MovieDiaries)
                .Include(m => m.Reviews);


            ViewBag.Movies = movies;
            
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

        [Authorize(Roles = "User,Editor,Admin")]

        public IActionResult Show(int id)
        {
            var movie = db.Movies
                .Include(m => m.Genre)
                .Include(m => m.User)
                .Include(m => m.ActorMovies).ThenInclude(am => am.Actor)
                .Include(m => m.MovieDiaries)
                .Include(m => m.Reviews).ThenInclude(r => r.User)
                .FirstOrDefault(m => m.Id == id);

            if (movie is null)
            {
                return NotFound();
            }
            SetAccessRights();
            return View(movie);
        }
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;
            if (User.IsInRole("Editor"))
            {
                ViewBag.AfisareButoane = true;
            }

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
        public IActionResult New(Movie movie)
        {
            var sanitizer = new HtmlSanitizer();

            movie.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                movie.Description = sanitizer.Sanitize(movie.Description);
                db.Movies.Add(movie);
                db.SaveChanges();
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
        public IActionResult Edit(int id, Movie requestMovie)
        {
            var sanitizer = new HtmlSanitizer();
            var movie = db.Movies.Find(id);

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
                movie.Genre = null;

                db.SaveChanges();

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