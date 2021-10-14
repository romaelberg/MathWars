using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathWars.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("TaskId")]
        public int TaskId { get; set; }
        
        [Required]
        public string Url { get; set; }
        
        [Required]
        public DateTime Created { get; set; }
    }
}