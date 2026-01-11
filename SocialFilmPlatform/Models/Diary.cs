using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocialFilmPlatform.Models
{
    public class Diary
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "List name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "List name must be between 3 and 100 characters")]
        public string Name { get; set; }
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
       
        public bool IsPublic { get; set; }
        
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<MovieDiary> MovieDiaries { get; set; } = [];
        public virtual ICollection<DiaryVote> DiaryVotes { get; set; } = [];
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}

