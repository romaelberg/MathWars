using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MathWars.Models
{
    public class WarTask
    {
        [Key]
        public int Id { get; set; }
        
        public AppUser Author { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Topic { get; set; }
        
        [Required]
        public string Body { get; set; }        
        
        public double Rating { get; set; }
        
        public DateTime Created { get; set; }
        
        public List<Tag> Tags { get; set; }
        
        public List<RightAnswer> RightAnswers { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Image> Images { get; set; }
        public List<SolveObj> SolvedWarTasks { get; set; }
    }
}