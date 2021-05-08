using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Entities;
using Core.ViewModels;

namespace BLL.Abstractions
{
    public interface ICourseService
    {
        public Task<ServiceResult> CreateAsync(CourseViewModel course);

        public Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCoursesAsync(string searchStr);
    }
}