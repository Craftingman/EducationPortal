using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Material : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string MaterialURL { get; set; }
        
        public ICollection<Course> Courses { get; set; }
        
        public ICollection<User> Users { get; set; }
    }
}