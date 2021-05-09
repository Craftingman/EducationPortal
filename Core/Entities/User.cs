using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;

namespace Core.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Surname { get; set; }

        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

        public virtual ICollection<Course> CreatedCourses { get; set; } = new List<Course>();

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

        public virtual ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();

        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

        public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }
}