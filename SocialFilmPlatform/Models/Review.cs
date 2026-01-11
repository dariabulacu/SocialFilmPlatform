using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace SocialFilmPlatform.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Review content is required")]
        [StringLength(2000, MinimumLength = 5, ErrorMessage = "Review must be between 5 and 2000 characters")]
        public string Content { get; set; }

        public DateTime DatePosted { get; set; }
        

        public int? MovieId { get; set; }
        //proprietatea de navigare

        //cheie externa (FK) - un review este postat de catre un user 

        public string? UserId { get; set; }

        //proprietatea de navigatie 
        //un review estse postat de catre un user 

        public virtual ApplicationUser? User { get; set; }
        public virtual Movie? Movie { get; set; }
        public virtual ICollection<ReviewVote> ReviewVotes { get; set; } = [];
        
        
    }
}

