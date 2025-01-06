using System.Collections;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Course : BaseEntity
    {
        public string Name { get; set; }
        
        public string Description { get; set; }

        public int? CreatorId { get; set; }

        public virtual User Creator { get; set; }

        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

        public virtual ICollection<User> Users { get; set; } = new List<User>();

        public virtual ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    }
}