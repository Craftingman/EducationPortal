namespace Core.Entities
{
    public class UserCourse : BaseEntity
    {
        public float Progress { get; set; }

        public bool IsCompleted { get; set; } = false;

        public int UserId { get; set; }

        public int CourseId { get; set; }
        
        public virtual User User { get; set; }
        
        public virtual Course Course { get; set; }
    }
}