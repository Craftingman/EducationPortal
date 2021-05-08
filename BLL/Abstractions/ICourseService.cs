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

        public Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCoursesAsync(string searchStr);

        public Task<ServiceResult<IEnumerable<SkillViewModel>>> GetCourseSkills(CourseViewModel courseShort);
    }
}