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

        public virtual ICollection<Skill> Skills { get; set; }
        
        public virtual ICollection<Material> Materials { get; set; }

        public virtual ICollection<User> Users { get; set; }
        
        public virtual ICollection<UserCourse> UserCourses { get; set; }
    }
}