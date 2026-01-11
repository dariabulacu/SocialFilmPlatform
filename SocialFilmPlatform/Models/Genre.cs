using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocialFilmPlatform.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Genre Name")]
        [Required(ErrorMessage = "Genre name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Genre name must be between 3 and 50 characters")]
        public string GenreName { get; set; }
        
        public virtual ICollection<Movie> Movies { get; set; } = [];
    }
}
