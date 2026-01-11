using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;
using Microsoft.AspNetCore.Authorization;

namespace SocialFilmPlatform.Controllers
{
    public class GenresController (ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext db = context;
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            var genres = from genre in db.Genres
                         orderby genre.GenreName
                         select genre;
            ViewBag.Genres = genres;
            return View();
        }
        public ActionResult Show(int id)
        {
            Genre? genre = db.Genres
                .Include(g=>g.Movies)
                .FirstOrDefault(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        public ActionResult New (Genre genre)
        {
            if (ModelState.IsValid)
            {
                db.Genres.Add(genre);
                db.SaveChanges();
                TempData["message"] = "The genre with name " + genre.GenreName + " has been added!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(genre);
            }
        }
        [Authorize(Roles = "Admin,Editor")]
        public ActionResult Edit (int id)
        {
            Genre? genre = db.Genres.Find(id);
            if (genre == null)
                return NotFound();
            return View(genre); 
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        public ActionResult Edit (int id, Genre requestGenre)
        {
            Genre? genre = db.Genres.Find(id);
            if(genre == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                genre.GenreName = requestGenre.GenreName;
                db.SaveChanges();
                TempData["message"] = "The genre has been modified!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestGenre);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        public ActionResult Delete(int id)
        {
            Genre ? genre = db.Genres.Include(g => g.Movies).FirstOrDefault(g => g.Id == id);
            if (genre is null)
            {
                return NotFound();
            }
            else{
                if (genre.Movies?.Any() == true)
                {
                    TempData["message"] = "The selected genre cannot be deleted while there are associated movies.";
                    return RedirectToAction("Index");   
                }
                db.Genres.Remove(genre);
                db.SaveChanges();
                TempData["message"] = "The genre has been deleted.";
                return RedirectToAction("Index");   
            }
        }
    }
}
