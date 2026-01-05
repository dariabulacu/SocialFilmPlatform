using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;

namespace SocialFilmPlatform.Controllers
{
    public class ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext db = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public IActionResult Index(int? movieId)
        {
            IQueryable<Review> reviewsQuery = db.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie);

            if (movieId.HasValue)
            {
                reviewsQuery = reviewsQuery.Where(r => r.MovieId == movieId.Value);
                var movie = db.Movies.Find(movieId.Value);
                ViewBag.MovieTitle = movie?.Title;
                ViewBag.MovieId = movieId.Value;
            }

            var reviews = reviewsQuery
                .OrderByDescending(r => r.DatePosted)
                .ToList();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(reviews);
        }

        [HttpPost]
        [Authorize]
        public IActionResult New(Review rev)
        {
            // Setează UserId din user-ul curent logat
            rev.UserId = _userManager.GetUserId(User);
            rev.DatePosted = DateTime.Now;
            
            // Regula: Un user nu poate lasa mai multe recenzii la acelasi film
            var existingReview = db.Reviews.FirstOrDefault(r => r.MovieId == rev.MovieId && r.UserId == rev.UserId);
            if (existingReview != null)
            {
                TempData["message"] = "Ai lăsat deja o recenzie pentru acest film. O poți edita pe cea existentă.";
                TempData["messageType"] = "alert-warning";
                 return RedirectToAction("Show", "Movies", new { id = rev.MovieId });
            }

            if (ModelState.IsValid)
            {
                db.Reviews.Add(rev);    
                db.SaveChanges();
                TempData["message"] = "Recenzia a fost adăugată!";
                TempData["messageType"] = "success";
                return RedirectToAction("Show", "Movies", new { id = rev.MovieId });
            }
            else
            {
                return RedirectToAction("Show", "Movies", new { id = rev.MovieId });
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int id)
        {
            Review rev = db.Reviews.Find(id);
            
            if (rev == null)
            {
                return NotFound();
            }

            // Verifică dacă user-ul curent este proprietarul review-ului SAU Admin/Editor
            var currentUserId = _userManager.GetUserId(User);
            if (rev.UserId != currentUserId && !User.IsInRole("Admin") && !User.IsInRole("Editor"))
            {
                TempData["message"] = "Nu aveți dreptul să ștergeți această recenzie.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", "Movies", new { id = rev.MovieId });
            }

            var movieId = rev.MovieId;
            db.Reviews.Remove(rev);
            db.SaveChanges();
            
            TempData["message"] = "Recenzia a fost ștearsă.";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Show", "Movies", new { id = movieId });
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            Review rev = db.Reviews.Find(id);
            
            if (rev == null)
            {
                return NotFound();
            }

            // Verifică dacă user-ul curent este proprietarul review-ului SAU Admin
            // Adminii/Editorii pot edita recenzii? De obicei moderatorii sterg, nu editeaza textul userului. 
            // Dar pentru consistenta, permitem Admin/Editor sa modifice daca e nevoie (cenzura partiala).
            var currentUserId = _userManager.GetUserId(User);
            if (rev.UserId != currentUserId && !User.IsInRole("Admin") && !User.IsInRole("Editor"))
            {
                TempData["message"] = "Nu aveți dreptul să editați această recenzie.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", "Movies", new { id = rev.MovieId });
            }

            return View(rev);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(int id, Review requestRev)
        {
            Review rev = db.Reviews.Find(id);
            
            if (rev == null)
            {
                return NotFound();
            }

            // Verifică dacă user-ul curent este proprietarul review-ului
            var currentUserId = _userManager.GetUserId(User);
            if (rev.UserId != currentUserId && !User.IsInRole("Admin") && !User.IsInRole("Editor"))
            {
                TempData["message"] = "Nu aveți dreptul să editați această recenzie.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", "Movies", new { id = rev.MovieId });
            }

            if (ModelState.IsValid)
            {
                rev.Content = requestRev.Content;
                db.SaveChanges();
                
                TempData["message"] = "Recenzia a fost actualizată.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Movies", new { id = rev.MovieId });
            }
            else
            {
                return View(requestRev);
            }
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult Vote(int reviewId, bool isLike)
        {
            var userId = _userManager.GetUserId(User);
            var existingVote = db.ReviewVotes.FirstOrDefault(v => v.ReviewId == reviewId && v.UserId == userId);

            if (existingVote != null)
            {
                if (existingVote.IsLike == isLike)
                {
                    // Toggle off if clicking the same vote again
                    db.ReviewVotes.Remove(existingVote);
                }
                else
                {
                    // Change vote type
                    existingVote.IsLike = isLike;
                    db.ReviewVotes.Update(existingVote);
                }
            }
            else
            {
                // New vote
                var vote = new ReviewVote
                {
                    ReviewId = reviewId,
                    UserId = userId,
                    IsLike = isLike
                };
                db.ReviewVotes.Add(vote);
            }

            db.SaveChanges();

            var likesCount = db.ReviewVotes.Count(v => v.ReviewId == reviewId && v.IsLike);
            var dislikesCount = db.ReviewVotes.Count(v => v.ReviewId == reviewId && !v.IsLike);
            
            // Re-fetch vote status for frontend update
            var currentVote = db.ReviewVotes.FirstOrDefault(v => v.ReviewId == reviewId && v.UserId == userId);
            var status = currentVote == null ? "none" : (currentVote.IsLike ? "like" : "dislike");

            return Json(new { success = true, likes = likesCount, dislikes = dislikesCount, userStatus = status });
        }
    }
}
