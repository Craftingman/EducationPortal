﻿using System.Collections;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Course : BaseEntity
    {
        public string Name { get; set; }
        
        public string Description { get; set; }

        public int? CreatorId { get; set; }

        public User Creator { get; set; }

        public ICollection<Skill> Skills { get; set; }
        
        public ICollection<Material> Materials { get; set; }

        public ICollection<User> Users { get; set; }
        
        public ICollection<UserCourse> UserCourses { get; set; }
    }
}