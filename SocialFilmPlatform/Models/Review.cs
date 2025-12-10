using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocialFilmPlatform.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string DatePosted { get; set; }
        
        //fk to movie
        public int? MovieId { get; set; }
        //proprietatea de navigare
        public virtual Movie? Movie { get; set; }
        
        
    }
}

