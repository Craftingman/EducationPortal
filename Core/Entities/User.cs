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

        public virtual ICollection<Material> Materials { get; set; }
        
        public virtual ICollection<Course> CreatedCourses { get; set; }
        
        public virtual ICollection<Course> Courses { get; set; }

        public virtual ICollection<UserCourse> UserCourses { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }

        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }
}