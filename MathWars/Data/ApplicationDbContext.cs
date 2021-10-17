using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MathWars.Models;

namespace MathWars.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<MathWars.Models.WarTask> WarTasks { get; set; }
        public DbSet<MathWars.Models.RightAnswer> RightAnswers { get; set; }
        public DbSet<MathWars.Models.Image> Images { get; set; }
        public DbSet<MathWars.Models.SolveObj> SolveHistory { get; set; }
        public DbSet<MathWars.Models.Comment> Comments { get; set; }
        public DbSet<MathWars.Models.Rate> Ratings { get; set; }
        public DbSet<MathWars.Models.Tag> Tags { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedWarTasks)
                .WithOne(w => w.Author)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<WarTask>()
                .HasMany(w => w.Tags)
                .WithOne(t => t.WarTask)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<WarTask>()
                .HasMany(w => w.RightAnswers)
                .WithOne(t => t.WarTask)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<WarTask>()
                .HasMany(w => w.Comments)
                .WithOne(t => t.WarTask)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<WarTask>()
                .HasMany(t => t.SolvedWarTasks)
                .WithOne(c => c.WarTask)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<WarTask>()
                .HasMany(w => w.Images)
                .WithOne(t => t.WarTask)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WarTask>()
                .HasIndex(b => new {b.Title, b.Body})
                .IsTsVectorExpressionIndex("english");
        }
    }
}