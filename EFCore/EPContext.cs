using System;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EFCore
{
    public class EPContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Material> Materials { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Video> Videos { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<UserCourse> UserCourses { get; set; }

        public DbSet<UserSkill> UserSkills { get; set; }

        public EPContext(DbContextOptions<EPContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Courses)
                .WithMany(c => c.Users)
                .UsingEntity<UserCourse>(
                    j => j
                        .HasOne(uc => uc.Course)
                        .WithMany(c => c.UserCourses)
                        .HasForeignKey(uc => uc.CourseId),
                    j => j
                        .HasOne(uc => uc.User)
                        .WithMany(u => u.UserCourses)
                        .HasForeignKey(uc => uc.UserId)
                );
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Skills)
                .WithMany(s => s.Users)
                .UsingEntity<UserSkill>(
                    j => j
                        .HasOne(us => us.Skill)
                        .WithMany(s => s.UserSkills)
                        .HasForeignKey(us => us.SkillId),
                    j => j
                        .HasOne(us => us.User)
                        .WithMany(u => u.UserSkills)
                        .HasForeignKey(uc => uc.UserId)
                );

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Creator)
                .WithMany(u => u.CreatedCourses);
        }
    }
}