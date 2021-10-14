using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathWars.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string AuthorId { get; set; }
        
        [Required]
        public string AuthorName { get; set; }
        
        [Required]
        public int TaskId { get; set; }
        
        [Required]
        public string Body { get; set; }
        
        [Required]
        public DateTime Created { get; set; }
    }
}