using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MathWars.Models
{
    public class Tag
    {
        [Key] 
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        public WarTask WarTask { get; set; }
    }
}