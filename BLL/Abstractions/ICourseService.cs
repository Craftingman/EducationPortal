using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Entities;
using Core.ViewModels;

namespace BLL.Abstractions
{
    public interface ICourseService
    {
        public Task<ServiceResult> CreateAsync(CourseViewModel courseShort, UserViewModel creator = null);

        public Task<ServiceResult> RemoveAsync(CourseViewModel courseShort);

        public Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCoursesAsync(string searchStr);

        public Task<ServiceResult<IEnumerable<SkillViewModel>>> GetCourseSkillsAsync(CourseViewModel courseShort);
        
        public Task<ServiceResult<IEnumerable<MaterialViewModel>>> GetCourseMaterialsAsync(CourseViewModel courseShort);

        public Task<ServiceResult> UpdateCourseInfoAsync(CourseViewModel courseShort);

        public Task<ServiceResult> AddMaterialAsync(MaterialViewModel materialShort);
        
        public Task<ServiceResult> RemoveMaterialAsync(MaterialViewModel materialShort);
        
        public Task<ServiceResult> AddSkillAsync(SkillViewModel materialShort);
        
        public Task<ServiceResult> RemoveSkillAsync(SkillViewModel materialShort);
    }
}