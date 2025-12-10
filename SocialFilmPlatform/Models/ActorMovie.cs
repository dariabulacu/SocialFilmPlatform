using System.ComponentModel.DataAnnotations.Schema;
namespace SocialFilmPlatform.Models
{
    public class ActorMovie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ActorId { get; set; }
        public int? MovieId { get; set; }

        public virtual Actor? Actor { get; set; }
        public virtual Movie? Movie { get; set; }
        
        public string Name { get; set; }
    }
}

