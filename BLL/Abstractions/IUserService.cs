using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Entities;
using Core.ViewModels;

namespace BLL.Abstractions
{
    public interface IUserService
    {
        Task<ServiceResult> RegisterUserAsync(UserViewModel userShort, string password);

        Task<ServiceResult<UserViewModel>> SignInUserAsync(string email, string password);

        Task<ServiceResult<bool>> HasRoleAsync(UserViewModel userShort, string role);

        Task<ServiceResult<Dictionary<CourseViewModel, float>>> GetCoursesAsync(int userId, string searchStr);
        
        Task<ServiceResult<Dictionary<CourseViewModel, float>>> GetInProgressCoursesAsync(int userId,  string searchStr);

        Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCreatedCoursesAsync(int userId,  string searchStr);
        
        Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCompletedCoursesAsync(int userId,  string searchStr);
    }
}