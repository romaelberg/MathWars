using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MathWars.Models;

namespace MathWars.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MathWars.Models.WarTask> WarTasks { get; set; }
        public DbSet<MathWars.Models.Tag> Tags { get; set; }
        public DbSet<MathWars.Models.RightAnswer> RightAnswers { get; set; }
        public DbSet<MathWars.Models.Image> Images { get; set; }
        public DbSet<MathWars.Models.SolveObj> SolveHistory { get; set; }
        public DbSet<MathWars.Models.Comment> Comments { get; set; }
        public DbSet<MathWars.Models.Rate> Ratings { get; set; }
    }
}