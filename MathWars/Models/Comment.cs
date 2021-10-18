using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MathWars.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        
        [JsonIgnore]
        public AppUser Author { get; set; }
        
        [Required]
        public int TaskId { get; set; }
        
        [Newtonsoft.Json.JsonIgnore]
        public WarTask WarTask { get; set; }
        
        [Required]
        public string Body { get; set; }
        
        [Required]
        public DateTime Created { get; set; }
        
        public List<Like> Likes { get; set; }
        public List<Dislike> Dislikes { get; set; }
    }
}