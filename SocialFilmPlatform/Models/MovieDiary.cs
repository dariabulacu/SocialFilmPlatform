using System.ComponentModel.DataAnnotations.Schema;
namespace SocialFilmPlatform.Models
{
    public class MovieDiary
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? MovieId { get; set; }
        public int? DiaryId { get; set; }

        public virtual Movie? Movie { get; set; }
        public virtual Diary? Diary { get; set; }
        
        public DateTime DiaryTime { get; set; }
        public string Name { get; set; }
        
    }
}

