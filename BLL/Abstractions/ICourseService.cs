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

        public Task<ServiceResult> RemoveAsync(int courseId);

        public Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCoursesAsync(string searchStr);

        public Task<ServiceResult<IEnumerable<SkillViewModel>>> GetCourseSkillsAsync(int courseId);
        
        public Task<ServiceResult<IEnumerable<MaterialViewModel>>> GetCourseMaterialsAsync(int courseId);

        public Task<ServiceResult> UpdateCourseInfoAsync(CourseViewModel courseShort);

        public Task<ServiceResult> AddMaterialAsync(int courseId, int materialId);
        
        public Task<ServiceResult> RemoveMaterialAsync(int courseId, int materialId);
        
        public Task<ServiceResult> AddSkillAsync(int courseId, int skillId);
        
        public Task<ServiceResult> RemoveSkillAsync(int courseId, int skillId);
    }
}