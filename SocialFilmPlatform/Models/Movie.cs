using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocialFilmPlatform.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        public float Score { get; set; }
        
        public DateTime ReleaseDate { get; set; }
        
        public virtual ICollection<Actor> Actors { get; set; } = [];
        
        public virtual ICollection<Review> Reviews { get; set; } = [];
        public virtual ICollection<MovieDiary> MovieDiaries { get; set; } = [];
        
        public virtual ICollection<ActorMovie> ActorMovies { get; set; } = [];
        
        //fk
        public int? GendreId { get; set; }
        //proprietatea de navigare

        //cheie externa (FK) - un film este postat de catre un user 

        public string? UserId { get; set; } 

        //proprietatea de navigatie 
        //un film estse postat de catre un user 

        public virtual ApplicationUser? User { get; set; }
        public virtual Genre? Genre { get; set; }
    }
}
