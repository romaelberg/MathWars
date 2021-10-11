using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace MathWars.Models
{
    public class AppUser: IdentityUser
    {
        public bool IsBanned { get; set; }
        public string SocialNetwork { get; set; }
        public string Avatar { get; set; }        
    }
}