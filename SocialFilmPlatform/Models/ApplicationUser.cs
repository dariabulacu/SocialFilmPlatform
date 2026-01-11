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

        public virtual ICollection<Review> Reviews { get; set; } = [];

        public virtual ICollection<Movie> Movies { get; set; } = [];

        public virtual ICollection<Diary> Diaries { get; set; } = [];

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles {  get; set; }

    }
}
