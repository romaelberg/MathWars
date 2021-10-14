using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathWars.Models
{
    public class Rate
    {
        [Key]
        public int Id { get; set; }
        
        // [ForeignKey("AppUserId")]
        // public int UserId { get; set; }
        [Required]
        public int TaskId { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public int Value { get; set; }
    }
}