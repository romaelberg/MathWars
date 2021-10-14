using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathWars.Models
{
    public class SolveObj
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("AppUserId")]
        public string UserId { get; set; }
        [Required]
        public string UserAnswer { get; set; }

        [ForeignKey("TaskId")] 
        public int TaskId { get; set; }
        
        public string Status { get; set; }
        [Required]
        public DateTime Created { get; set; }
    }
}