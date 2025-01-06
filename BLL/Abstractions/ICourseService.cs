using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Entities;
using Core.ViewModels;

namespace BLL.Abstractions
{
    public interface ICourseService
    {
        Task<ServiceResult> CreateAsync(CourseViewModel courseShort, UserViewModel creator = null);

        Task<ServiceResult> RemoveAsync(int courseId);

        Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCoursesAsync(string searchStr);

        Task<ServiceResult<IEnumerable<SkillViewModel>>> GetCourseSkillsAsync(int courseId);
        
        Task<ServiceResult<IEnumerable<MaterialViewModel>>> GetCourseMaterialsAsync(int courseId);

        Task<ServiceResult> UpdateCourseInfoAsync(CourseViewModel courseShort);

        Task<ServiceResult> AddMaterialAsync(int courseId, int materialId);
        
        Task<ServiceResult> RemoveMaterialAsync(int courseId, int materialId);
        
        Task<ServiceResult> AddSkillAsync(int courseId, int skillId);
        
        Task<ServiceResult> RemoveSkillAsync(int courseId, int skillId);

        Task<ServiceResult<bool>> HasSkillAsync(int courseId, int skillId);
        
        Task<ServiceResult<bool>> HasMaterialAsync(int courseId, int materialId);
    }
}