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

        public ICollection<Material> Materials { get; set; }

        public ICollection<Course> Courses { get; set; }

        public ICollection<UserCourse> UserCourses { get; set; }

        public ICollection<Skill> Skills { get; set; }

        public ICollection<UserSkill> UserSkills { get; set; }
    }
}