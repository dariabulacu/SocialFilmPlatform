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
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        
        //fk to movie
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

