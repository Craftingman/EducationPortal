using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Entities;
using Core.ViewModels;

namespace BLL.Abstractions
{
    public interface IUserService
    {
        Task<ServiceResult> RegisterUserAsync(UserViewModel userShort, string password, string role = "standard");

        Task<ServiceResult<UserViewModel>> SignInUserAsync(string email, string password);

        Task<ServiceResult<bool>> HasRoleAsync(UserViewModel userShort, string role);
        
        Task<ServiceResult<bool>> UserExistsAsync(string email);
        
        Task<ServiceResult> CompleteCourseAsync(int userId, int courseId);

        Task<ServiceResult> CompleteMaterialAsync(int userId, int materialId);

        Task<ServiceResult> AddUserCourse(int userId, int courseId);

        Task<ServiceResult> AddUserSkill(int userId, int skillId);
        
        Task<ServiceResult> RemoveUserCourse(int userId, int courseId);

        Task<ServiceResult> UpdateUserCourses(int userId);
        
        Task<ServiceResult> UpdateUserCourses(int userId, int materialId);
        
        Task<ServiceResult> UpdateCourse(int userId, int courseId);

        Task<ServiceResult<Dictionary<CourseViewModel, float>>> GetCoursesAsync(int userId, string searchStr);
        
        Task<ServiceResult<Dictionary<CourseViewModel, float>>> GetActiveCoursesAsync(int userId,  string searchStr);

        Task<ServiceResult<ActiveCourseViewModel>> GetActiveCourseAsync(int userId, int courseId);

        Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCreatedCoursesAsync(int userId,  string searchStr);
        
        Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCompletedCoursesAsync(int userId,  string searchStr);
    }
}