using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SocialFilmPlatform.Models
{
    public class ApplicationUser : IdentityUser 
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        //un user poate sa lase un singur review per film, dar le poate edita sau sterge
        //de asemenea poate sa dea like la review-ul altui user 
        public virtual ICollection<Review> Reviews { get; set; } = [];

        //un user poate posta mai multe filme (doar adminii)
        public virtual ICollection<Movie> Movies { get; set; } = [];

        //un user poate sa creeze mai multe liste cu filme whatever 
        public virtual ICollection<Diary> Diaries { get; set; } = [];

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles {  get; set; }

    }
}
