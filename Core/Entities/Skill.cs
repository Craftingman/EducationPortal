using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Skill : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        
        public virtual ICollection<Course> Courses { get; set; }
        
        public virtual ICollection<User> Users { get; set; }
        
        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }
}