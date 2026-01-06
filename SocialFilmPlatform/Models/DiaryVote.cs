using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialFilmPlatform.Models
{
    public class DiaryVote
    {
        [Key]
        public int Id { get; set; }

        public int DiaryId { get; set; }
        public virtual Diary Diary { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public DateTime VoteDate { get; set; } = DateTime.Now;
    }
}
