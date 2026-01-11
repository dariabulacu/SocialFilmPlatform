using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialFilmPlatform.Models
{
    public class ReviewVote
    {
        [Key]
        public int Id { get; set; }

        public int ReviewId { get; set; }
        public virtual Review Review { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        public bool IsLike { get; set; }
    }
}
