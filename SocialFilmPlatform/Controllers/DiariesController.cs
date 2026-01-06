using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;
using Ganss.Xss;

namespace SocialFilmPlatform.Controllers
{
    public class DiariesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly ApplicationDbContext db = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        // Public Index: Shows all public diaries or own private diaries mixed? 
        // Requirement: "Orice utilizator ... poate vizualiza bookmark-urile publice"
        // Also "Cauta si paginare"
        public IActionResult Index()
        {
            var search = "";
            var sortOrder = HttpContext.Request.Query["sort"].ToString(); // "recent" or "popular"
            
            // Base query: All Public Diaries
            IQueryable<Diary> diaries = db.Diaries
                                           .Include(d => d.User)
                                           .Include(d => d.MovieDiaries)
                                           .Include(d => d.DiaryVotes)
                                           .Where(d => d.IsPublic);

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                diaries = diaries.Where(d => d.Name.Contains(search) || (d.Description != null && d.Description.Contains(search)));
            }

            // Sorting
            if (sortOrder == "popular")
            {
                diaries = diaries.OrderByDescending(d => d.DiaryVotes.Count());
            }
            else // Default to Recent
            {
                diaries = diaries.OrderByDescending(d => d.CreatedAt);
            }

            ViewBag.SearchString = search;
            ViewBag.SortOrder = sortOrder;

            // Pagination
            int _perPage = 6;
            int totalItems = diaries.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedDiaries = diaries.Skip(offset).Take(_perPage).ToList();
            
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Diaries = paginatedDiaries;
             
            // Pagination Base URL building
            var baseUrl = "/Diaries/Index?";
            if (!string.IsNullOrEmpty(search)) baseUrl += $"search={search}&";
            if (!string.IsNullOrEmpty(sortOrder)) baseUrl += $"sort={sortOrder}&";
            ViewBag.PaginationBaseUrl = baseUrl + "page=";

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        [Authorize]
        public IActionResult MyDiaries()
        {
            var currentUserId = _userManager.GetUserId(User);
            var diaries = db.Diaries
                            .Where(d => d.UserId == currentUserId)
                            .Include(d => d.User)
                            .Include(d => d.MovieDiaries)
                            .Include(d => d.DiaryVotes)
                            .OrderByDescending(d => d.CreatedAt)
                            .ToList();
            
            ViewBag.Diaries = diaries;
            return View();
        }

        public IActionResult Show(int id)
        {
            var diary = db.Diaries
                .Include(d => d.User)
                .Include(d => d.MovieDiaries).ThenInclude(md => md.Movie)
                .Include(d => d.DiaryVotes)
                .FirstOrDefault(d => d.Id == id);

            if (diary is null)
            {
                return NotFound();
            }

            // Privacy check
            var currentUserId = _userManager.GetUserId(User);
            if (!diary.IsPublic && diary.UserId != currentUserId && !User.IsInRole("Admin")) 
            {
                return Forbid();
            }

            ViewBag.CurrentUser = currentUserId;
            ViewBag.IsAdmin = User.IsInRole("Admin");
            ViewBag.UserHasVoted = false;
            
            if (currentUserId != null)
            {
                ViewBag.UserHasVoted = diary.DiaryVotes.Any(dv => dv.UserId == currentUserId);
            }

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(diary);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        [HttpPost]
        public IActionResult Vote(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var diary = db.Diaries.Include(d => d.DiaryVotes).FirstOrDefault(d => d.Id == id);

            if (diary == null) return NotFound();
            if (!diary.IsPublic) return Forbid(); // Should voting be allowed on private lists? Probably not if they can't see them.

            var existingVote = db.DiaryVotes.FirstOrDefault(dv => dv.DiaryId == id && dv.UserId == currentUserId);

            if (existingVote != null)
            {
                db.DiaryVotes.Remove(existingVote);
            }
            else
            {
                var vote = new DiaryVote
                {
                    DiaryId = id,
                    UserId = currentUserId,
                    VoteDate = DateTime.Now
                };
                db.DiaryVotes.Add(vote);
            }
            db.SaveChanges();
            
            return RedirectToAction("Show", new { id = id });
        }


        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New(Diary diary)
        {
            diary.UserId = _userManager.GetUserId(User);
            diary.CreatedAt = DateTime.Now;
            
            var sanitizer = new HtmlSanitizer();
            if (diary.Description != null) diary.Description = sanitizer.Sanitize(diary.Description);
            if (diary.Content != null) diary.Content = sanitizer.Sanitize(diary.Content);

            if (ModelState.IsValid)
            {
                db.Diaries.Add(diary);
                db.SaveChanges();
                TempData["message"] = "List created successfully!";
                TempData["messageType"] = "success";
                return RedirectToAction("MyDiaries"); // Redirect to My Lists
            }
            return View(diary);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            var diary = db.Diaries.Find(id);
            if (diary is null)
            {
                return NotFound();
            }
            
            var currentUserId = _userManager.GetUserId(User);
            if (diary.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                 TempData["message"] = "You cannot edit this list!";
                 TempData["messageType"] = "alert-danger";
                 return RedirectToAction("Index");
            }

            return View(diary);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Diary requestDiary)
        {
            var diary = db.Diaries.Find(id);
            if (diary is null)
            {
                return NotFound();
            }
            
            var currentUserId = _userManager.GetUserId(User);
            if (diary.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                 return Forbid();
            }

            var sanitizer = new HtmlSanitizer();

            if (ModelState.IsValid)
            {
                diary.Name = requestDiary.Name;
                diary.Description = requestDiary.Description != null ? sanitizer.Sanitize(requestDiary.Description) : null;
                diary.Content = requestDiary.Content != null ? sanitizer.Sanitize(requestDiary.Content) : null;
                diary.IsPublic = requestDiary.IsPublic;
                
                db.SaveChanges();
                TempData["message"] = "List updated successfully!";
                TempData["messageType"] = "success";
                return RedirectToAction("MyDiaries");
            }
            return View(requestDiary);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Delete(int id)
        {
            var diary = db.Diaries.Find(id);
            if (diary is null)
            {
                return NotFound();
            }
            
            var currentUserId = _userManager.GetUserId(User);
            if (diary.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                 TempData["message"] = "You cannot delete this list!";
                 TempData["messageType"] = "alert-danger";
                 return RedirectToAction("Index");
            }
            
            db.Diaries.Remove(diary);
            db.SaveChanges();
            TempData["message"] = "List deleted successfully!";
            TempData["messageType"] = "success";
            return RedirectToAction("MyDiaries");
        }
    }
}
