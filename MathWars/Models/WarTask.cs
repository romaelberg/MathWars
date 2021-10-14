using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MathWars.Models
{
    public class WarTask
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("AppUserId")]
        public string AuthorId { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Topic { get; set; }
        
        [Required]
        public string Body { get; set; }        
        
        [Required]
        public double Rating { get; set; }
        
        [Required]
        public DateTime Created { get; set; }
    }
}