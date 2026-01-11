using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocialFilmPlatform.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Film title is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Director name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Director name must be between 2 and 100 characters")]
        public string Director { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Description must be at least 10 characters long")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Score is required")]
        [Range(1, 10, ErrorMessage = "Score must be between 1 and 10")]
        public float Score { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        public DateTime ReleaseDate { get; set; }

        public string? ImageUrl { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? TrailerUrl { get; set; }
        
        public virtual ICollection<Actor> Actors { get; set; } = [];
        
        public virtual ICollection<Review> Reviews { get; set; } = [];
        public virtual ICollection<MovieDiary> MovieDiaries { get; set; } = [];
        
        public virtual ICollection<ActorMovie> ActorMovies { get; set; } = [];
        
        //fk
        [Required(ErrorMessage = "Please select a genre")]
        public int? GenreId { get; set; }
        //proprietatea de navigare

        //cheie externa (FK) - un film este postat de catre un user 

        public string? UserId { get; set; } 

        //proprietatea de navigatie 
        //un film estse postat de catre un user 

        public virtual ApplicationUser? User { get; set; }
        public virtual Genre? Genre { get; set; }
    }
}
