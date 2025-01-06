namespace Core.ViewModels
{
    public class CourseViewModel : EntityViewModel
    {
        public string Name { get; set; }
        
        public string Description { get; set; }

        public int? CreatorId { get; set; }
    }
}