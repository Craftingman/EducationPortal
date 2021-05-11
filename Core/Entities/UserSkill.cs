namespace Core.Entities
{
    public class UserSkill : BaseEntity
    {
        public int Level { get; set; } = 1;

        public int UserId { get; set; }

        public int SkillId { get; set; }
        
        public virtual User User { get; set; }
        
        public virtual Skill Skill { get; set; }
    }
}