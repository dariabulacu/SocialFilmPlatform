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
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Review rev = db.Reviews.Find(id);
            db.Reviews.Remove(rev);
            db.SaveChanges();
            return RedirectToAction("/Movies/Show/" + rev.MovieId);
        }

        public IActionResult Edit(int id)
        {
            Review rev = db.Reviews.Find(id);
            return View(rev);
        }

        [HttpPost]

        public IActionResult Edit (int id, Review requestRev)
        {
            Review rev = db.Reviews.Find(id);
            if (ModelState.IsValid)
            {
                rev.Content = requestRev.Content;
                db.SaveChanges();
                return Redirect("/Movies/Show/" + rev.MovieId);
            }else
            {
                return View(requestRev);
            }
        }
    }
}
