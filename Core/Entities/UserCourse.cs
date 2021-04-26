namespace Core.Entities
{
    public class UserCourse : BaseEntity
    {
        public float Progress { get; set; }

        public int UserId { get; set; }

        public int CourseId { get; set; }
        
        public User User { get; set; }
        
        public Course Course { get; set; }
    }
}