using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.pragma 
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<User>()
                .HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
                
                modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();




        }
    }
}

