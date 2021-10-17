using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathWars.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        
        public AppUser Author { get; set; }
        
        [Required]
        public int TaskId { get; set; }
        
        public WarTask WarTask { get; set; }
        
        [Required]
        public string Body { get; set; }
        
        [Required]
        public DateTime Created { get; set; }
    }
}