using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks{ get; set; }

        public DbSet<Role> Roles{get; set;}
        public DbSet<RefreshToken> RefreshTokens{get; set;}

    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<User>()
                .HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                 
                 modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);



                modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
                
                modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

                modelBuilder.Entity<Role>()
                .HasIndex(R=>R.Name)
                .IsUnique();

                modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

                modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Full system access - can manage users and all tasks",
                    CreatedAt = new DateTime(2026, 3, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = 2,
                    Name = "Manager",
                    Description = "Can view all tasks but only modify own tasks",
                    CreatedAt = new DateTime(2026, 3, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = 3,
                    Name = "User",
                    Description = "Can only manage own tasks",
                    CreatedAt = new DateTime(2026, 3, 11, 0, 0, 0, DateTimeKind.Utc)
                }
            );






        }
    }
}

