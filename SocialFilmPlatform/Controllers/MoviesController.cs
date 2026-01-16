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


        public IActionResult Index()
        {
            var movies = db.Movies
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
            if (!string.IsNullOrWhiteSpace(HttpContext.Request.Query["search"]))
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                
                movies = movies.Where(m => 
                    m.Title.Contains(search) || 
                    m.Director.Contains(search) ||
                    m.Description.Contains(search) ||
                    (m.Genre != null && m.Genre.GenreName.Contains(search))
                );
            }
            

            var sortOrder = HttpContext.Request.Query["sort"].ToString();
            if (string.IsNullOrEmpty(sortOrder)) sortOrder = "recent";

            if (sortOrder == "popular")
            {
                movies = movies.OrderByDescending(m => m.Score)
                               .ThenByDescending(m => m.Reviews.Count)
                               .ThenByDescending(m => m.Id);
            }
            else 
            {
                movies = movies.OrderByDescending(m => m.ReleaseDate)
                               .ThenByDescending(m => m.Id);
            }

            ViewBag.SearchString = search;
            ViewBag.SortOrder = sortOrder;

            int _perPage = 6;
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
            

            var baseUrl = "/Movies/Index?";
            if (!string.IsNullOrEmpty(search)) baseUrl += $"search={search}&";
            if (!string.IsNullOrEmpty(sortOrder)) baseUrl += $"sort={sortOrder}&";
            ViewBag.PaginationBaseUrl = baseUrl + "page=";

            return View();
        }

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

            TempData["message"] = "You do not have permission to modify a movie that does not belong to you.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Delete(int id)
        {
            var movie = db.Movies
                .Include(m => m.ActorMovies)
                .Include(m => m.Reviews)
                .Include(m => m.MovieDiaries)
                .FirstOrDefault(m => m.Id == id);

            if (movie is null)
            {
                return NotFound();
            }
            else
            {
                if(movie.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    if (movie.ActorMovies != null && movie.ActorMovies.Any())
                    {
                        db.Set<ActorMovie>().RemoveRange(movie.ActorMovies);
                    }

                    if (movie.Reviews != null && movie.Reviews.Any())
                    {
                        foreach (var review in movie.Reviews)
                        {
                            var reviewVotes = db.Set<ReviewVote>().Where(rv => rv.ReviewId == review.Id);
                            db.Set<ReviewVote>().RemoveRange(reviewVotes);
                        }
                        db.Reviews.RemoveRange(movie.Reviews);
                    }

                    if (movie.MovieDiaries != null && movie.MovieDiaries.Any())
                    {
                        db.MovieDiaries.RemoveRange(movie.MovieDiaries);
                    }

                    db.Movies.Remove(movie);
                    db.SaveChanges();

                    TempData["message"] = "Movie deleted.";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "You do not have permission to delete a movie that does not belong to you.";
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

                await db.SaveChangesAsync();

                TempData["message"] = "Movie updated.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            TempData["message"] = "You do not have permission to modify a movie that does not belong to you.";
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