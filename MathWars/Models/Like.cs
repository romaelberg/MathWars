using System.ComponentModel.DataAnnotations;

namespace MathWars.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }
        
        public Comment Comment { get; set; }
        public AppUser User { get; set; }
    }
}