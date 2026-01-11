using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SocialFilmPlatform.Models
{
    public class ApplicationUser : IdentityUser 
    {
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string? LastName { get; set; }

        public string? ProfilePictureUrl { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

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
