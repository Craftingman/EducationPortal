using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Skill : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        
        public ICollection<Course> Courses { get; set; }
        
        public ICollection<User> Users { get; set; }
        
        public ICollection<UserSkill> UserSkills { get; set; }
    }
}