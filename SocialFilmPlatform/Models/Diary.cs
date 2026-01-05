using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocialFilmPlatform.Models
{
    public class Diary
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
       
        public bool IsPublic { get; set; }
        
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<MovieDiary> MovieDiaries { get; set; } = [];
    }
}

