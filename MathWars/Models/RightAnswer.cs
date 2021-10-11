using System.ComponentModel.DataAnnotations;


namespace MathWars.Models
{
    public class RightAnswer
    {
        [Key]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Answer { get; set; }
    }
}