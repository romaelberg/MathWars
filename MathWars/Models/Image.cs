using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathWars.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Task")]
        public int TaskId { get; set; }
        
        [Required]
        public string Url { get; set; }
    }
}