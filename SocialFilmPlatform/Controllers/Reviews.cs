using Microsoft.AspNetCore.Mvc;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;

namespace SocialFilmPlatform.Controllers
{
    public class ReviewsController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext db = context;

        [HttpPost]
        public IActionResult New(Review rev)
        {
            rev.DatePosted = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Reviews.Add(rev);    
                db.SaveChanges();
                return RedirectToAction("/Movies/Show/" + rev.MovieId);
            }
            else
            {
                return RedirectToAction("/Movies/Show/" + rev.MovieId);
            }
        }
    }
}
