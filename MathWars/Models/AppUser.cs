using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MathWars.Models;


namespace MathWars.Models
{
    public class AppUser: IdentityUser
    {
        public bool IsBanned { get; set; }
        public List<WarTask> CreatedWarTasks { get; set; }
        public List<WarTask> SolvedWarTasks { get; set; }
        public List<Like> Likes { get; set; }
        public List<Dislike> DisLikes { get; set; }
        public List<Comment> Comments { get; set; }
        public string Avatar { get; set; }        
    }
}