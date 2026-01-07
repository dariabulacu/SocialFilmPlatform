using System.ComponentModel.DataAnnotations;

namespace SocialFilmPlatform.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public bool IsPublic { get; set; } = true;

        public virtual ICollection<Diary> Diaries { get; set; } = new List<Diary>();
    }
}
