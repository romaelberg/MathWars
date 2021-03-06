using System;
using System.ComponentModel.DataAnnotations;


namespace MathWars.Models
{
    public class RightAnswer
    {
        [Key]
        public int Id { get; set; }
        public WarTask WarTask { get; set; }
        public string Answer { get; set; }
        [Required]
        public DateTime Created { get; set; }
    }
}