using System.ComponentModel.DataAnnotations;

namespace SocialFilmPlatform.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name length must be between 2 and 50 chars")]
        public string Name { get; set; }

        public bool IsPublic { get; set; } = true;

        public virtual ICollection<Diary> Diaries { get; set; } = new List<Diary>();
    }
}
