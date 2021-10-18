using System.ComponentModel.DataAnnotations;

namespace MathWars.Models
{
    public class Dislike
    {
        [Key]
        public int Id { get; set; }
        
        public Comment Comment { get; set; }
        public AppUser User { get; set; }

    }
}