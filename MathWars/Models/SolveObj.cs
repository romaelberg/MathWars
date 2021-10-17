using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathWars.Models
{
    public class SolveObj
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserAnswer { get; set; }
        
        public string Status { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public AppUser Author { get; set; }
        public WarTask WarTask { get; set; }
        
    }
}