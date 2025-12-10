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
        public string ReleaseDate { get; set; }
        
        public virtual ICollection<Actor> Actors { get; set; } = [];
        
        public virtual ICollection<Review> Reviews { get; set; } = [];
        public virtual ICollection<MovieDiary> MovieDiaries { get; set; } = [];
        
        public virtual ICollection<ActorMovie> ActorMovies { get; set; } = [];
        
        //fk
        public int? GendreId { get; set; }
        //proprietatea de navigare
        public virtual Genre? Genre { get; set; }
    }
}
