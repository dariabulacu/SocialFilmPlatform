using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocialFilmPlatform.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Actor name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Biography/Description is required")]
        [MinLength(10, ErrorMessage = "Biography must be at least 10 characters")]
        public string Description { get; set; }

        public string? PhotoUrl { get; set; }
        
        public virtual ICollection<ActorMovie> ActorMovies { get; set; } = [];
        
    }
}

