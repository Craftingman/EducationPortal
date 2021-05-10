using System.Collections;
using System.Collections.Generic;

namespace Core.ViewModels
{
    public class ActiveCourseViewModel : EntityViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public float Progress { get; set; }

        public ICollection<MaterialViewModel> CompletedMaterials { get; set; }
        
        public ICollection<MaterialViewModel> UncompletedMaterials { get; set; }
    }
}