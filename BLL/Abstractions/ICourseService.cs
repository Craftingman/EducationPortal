using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Entities;

namespace BLL.Abstractions
{
    public interface ICourseService
    {
        public Task<ServiceResult> CreateAsync(Course course);

        public Task<ServiceResult<IEnumerable<Course>>> GetAllAsync();
        
        public Task<ServiceResult<IEnumerable<Course>>> FindAsync();
    }
}