using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Abstractions;
using Core;
using Core.Entities;
using Core.ViewModels;
using DAL.Abstractions;
using EFCore;
using Microsoft.AspNetCore.Identity;

namespace BLL
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole<int>> _roleManager;

        private readonly IMapper _mapper;

        private readonly IRepositoryBase<User> _userRepository;
        
        private readonly IRepositoryBase<Course> _courseRepository;
        
        private readonly IRepositoryBase<UserCourse> _userCourseRepository;
        
        private readonly IRepositoryBase<Material> _materialRepository;
        
        private readonly IRepositoryBase<Skill> _skillRepository;

        private readonly IUserValidator<User> _userValidator;
        
        private readonly IPasswordValidator<User> _passwordValidator;

        public UserService(
            UserManager<User> userManager, 
            RoleManager<IdentityRole<int>> roleManager,
            IRepositoryBase<User> userRepository,
            IRepositoryBase<Course> courseRepository,
            IRepositoryBase<Material> materialRepository,
            IRepositoryBase<UserCourse> userCourseRepository,
            IRepositoryBase<Skill> skillRepository,
            IUserValidator<User> userValidator,
            IPasswordValidator<User> passwordValidator,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _materialRepository = materialRepository;
            _courseRepository = courseRepository;
            _userCourseRepository = userCourseRepository;
            _skillRepository = skillRepository;
            _userValidator = userValidator;
            _passwordValidator = passwordValidator;
            _mapper = mapper;
        }

        public async Task<ServiceResult> RegisterUserAsync(UserViewModel userShort, string password, string role)
        {
            try
            {
                User user = _mapper.Map<User>(userShort);
            
                var validatePasswordResult = await _passwordValidator.ValidateAsync(_userManager, user, password);
                var validateUserResult = await _userValidator.ValidateAsync(_userManager, user);

                if (!validatePasswordResult.Succeeded)
                {
                    return ServiceResult.CreateFailure(validatePasswordResult.Errors.FirstOrDefault()?.Description, 401);
                }
                if (!validateUserResult.Succeeded)
                {
                    return ServiceResult.CreateFailure(validateUserResult.Errors.FirstOrDefault()?.Description, 401);
                }

                var createResult = await _userManager.CreateAsync(user, password);
                var addRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (createResult.Succeeded && addRoleResult.Succeeded)
                {
                    return ServiceResult.CreateSuccessResult();
                }

                string errorString = String.Concat(
                    createResult.Errors.Select(e => e.ToString()),
                    addRoleResult.Errors.Select(e => e.ToString()));

                return ServiceResult.CreateFailure(errorString);
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<UserViewModel>> SignInUserAsync(string email, string password)
        {
            try
            {
                User user = await Task.Run(() => _userManager.Users.FirstOrDefault(u => u.Email == email));

                if (user == null)
                {
                    return ServiceResult<UserViewModel>.CreateFailure("Wrong credentials.", 401);
                }
            
                var result = _userManager.PasswordHasher.VerifyHashedPassword(
                    user, user.PasswordHash, password);
                if (result == PasswordVerificationResult.Success)
                {
                    UserViewModel userShort = _mapper.Map<UserViewModel>(user);
                
                    return ServiceResult<UserViewModel>.CreateSuccessResult(userShort);
                }
            
                return ServiceResult<UserViewModel>.CreateFailure("Wrong credentials.", 401);
            }
            catch (Exception e)
            {
                return ServiceResult<UserViewModel>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<bool>> HasRoleAsync(UserViewModel userShort, string role)
        {
            try
            {
                if (userShort is null)
                {
                    return ServiceResult<bool>.CreateSuccessResult(false);
                }
                
                User user = await Task.Run(() => _userManager.Users.FirstOrDefault(u => u.Id == userShort.Id));
            
                if (user is null)
                {
                    string message = $"User with id {userShort.Id} doesn't exist.";
                    
                    return ServiceResult<bool>.CreateFailure(message, 404);
                }
            
                bool result = await _userManager.IsInRoleAsync(user, role);

                return ServiceResult<bool>.CreateSuccessResult(result);
            }
            catch (Exception e)
            {
                return ServiceResult<bool>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<bool>> UserExistsAsync(string email)
        {
            try
            {
                User user = await Task.Run(() => _userManager.Users.FirstOrDefault(u => u.Email == email));

                bool result = !(user is null);

                return ServiceResult<bool>.CreateSuccessResult(result);
            }
            catch (Exception e)
            {
                return ServiceResult<bool>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> CompleteCourseAsync(int userId, int courseId)
        {
            try
            {
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                User user = userResult.Result;

                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }

                UserCourse userCourse = user.UserCourses.FirstOrDefault(uc => uc.CourseId == courseId);
            
                if (userCourse is null)
                {
                    string message = $"User with id {userId} does not have course with id {courseId}.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }

                foreach (Skill skill in userCourse.Course.Skills)
                {
                    var result = await AddUserSkill(userId, skill.Id);

                    if (!result.Success)
                    {
                        return ServiceResult.CreateFailure("Database error."); 
                    }
                }

                userCourse.IsCompleted = true;
                
                var saveResult = await _userRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> CompleteMaterialAsync(int userId, int materialId)
        {
            try
            {
                var materialResult = await _materialRepository.FindAsync(materialId);
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success || !materialResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                User user = userResult.Result;
                Material material = materialResult.Result;
                
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }
            
                if (material is null)
                {
                    string message = $"Material with id {materialId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }
                
                user.Materials.Add(material);
                
                var saveResult = await _userRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                var updateResult = await UpdateUserCourses(userId, materialId);
                if (!updateResult.Success)
                {
                    return ServiceResult.CreateFailure(updateResult.NonSuccessMessage);
                }
                
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddUserCourse(int userId, int courseId)
        {
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseId);
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success || !courseResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                User user = userResult.Result;
                Course course = courseResult.Result;
                
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }
            
                if (course is null)
                {
                    string message = $"Course with id {courseId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }
                
                user.Courses.Add(course);
                
                var saveResult = await _userRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }
                
                var updateResult = await UpdateCourse(userId, courseId);
                if (!updateResult.Success)
                {
                    return ServiceResult.CreateFailure(updateResult.NonSuccessMessage);
                }
                
                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> AddUserSkill(int userId, int skillId)
        {
            try
            {
                var skillResult = await _skillRepository.FindAsync(skillId);
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success || !skillResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                User user = userResult.Result;
                Skill skill = skillResult.Result;
                
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }
            
                if (skill is null)
                {
                    string message = $"Skill with id {skillId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }

                UserSkill userSkill = user.UserSkills.FirstOrDefault(us => us.SkillId == skillId);

                if (userSkill is not null)
                {
                    userSkill.Level++;
                }
                else
                {
                    user.Skills.Add(skill);
                }

                var saveResult = await _userRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> RemoveUserCourse(int userId, int courseId)
        {
            try
            {
                var courseResult = await _courseRepository.FindAsync(courseId);
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success || !courseResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                User user = userResult.Result;
                Course course = courseResult.Result;
                
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }
            
                if (course is null)
                {
                    string message = $"Course with id {userId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }
                
                user.Courses.Remove(user.Courses.FirstOrDefault(c => c.Id == courseId));
                
                var saveResult = await _userRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> UpdateUserCourses(int userId)
        {
            try
            {
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                User user = userResult.Result;
                
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }

                List<UserCourse> userCourses = user.UserCourses.ToList();

                foreach (UserCourse userCourse in userCourses)
                {
                    int completedCourseMaterialsCount = user.Materials
                        .Join(userCourse.Course.Materials,
                            um => um.Id,
                            cm => cm.Id, 
                            (um, cm) => um)
                        .Count();

                    if (completedCourseMaterialsCount == 0)
                    {
                        userCourse.Progress = 0;
                    }
                    else
                    {
                        userCourse.Progress = completedCourseMaterialsCount / (float)userCourse.Course.Materials.Count;
                    }
                }

                var saveResult = await _userRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> UpdateUserCourses(int userId, int materialId)
        {
            try
            {
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                User user = userResult.Result;
                
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }

                List<UserCourse> userCourses = user.UserCourses
                    .Where(uc => uc.Course.Materials
                        .Select(m => m.Id).Contains(materialId))
                    .ToList();

                foreach (UserCourse userCourse in userCourses)
                {
                    int completedCourseMaterialsCount = user.Materials
                        .Join(userCourse.Course.Materials,
                            um => um.Id,
                            cm => cm.Id, 
                            (um, cm) => um)
                        .Count();

                    if (completedCourseMaterialsCount == 0)
                    {
                        userCourse.Progress = 0;
                    }
                    else
                    {
                        userCourse.Progress = completedCourseMaterialsCount / (float)userCourse.Course.Materials.Count;
                    }
                }

                var saveResult = await _userRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult> UpdateCourse(int userId, int courseId)
        {
            try
            {
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                User user = userResult.Result;
                
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }

                UserCourse userCourse = user.UserCourses.FirstOrDefault(uc => uc.CourseId == courseId);

                if (userCourse is null)
                {
                    string message = $"User with id {userId} doesn't have course with id {courseId}.";
                    
                    return ServiceResult.CreateFailure(message, 404);
                }

                int completedCourseMaterialsCount = user.Materials
                    .Join(userCourse.Course.Materials,
                        um => um.Id,
                        cm => cm.Id, 
                        (um, cm) => um)
                    .Count();

                if (completedCourseMaterialsCount == 0)
                {
                    userCourse.Progress = 0;
                }
                else
                {
                    userCourse.Progress = completedCourseMaterialsCount / (float)userCourse.Course.Materials.Count;
                }

                var saveResult = await _userRepository.SaveAsync();
                if (!saveResult.Success)
                {
                    return ServiceResult.CreateFailure("Database error.");
                }

                return ServiceResult.CreateSuccessResult();
            }
            catch (Exception e)
            {
                return ServiceResult.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<Dictionary<CourseViewModel, float>>> GetCoursesAsync
            (int userId, string searcStr = "")
        {
            try
            {
                User user = await Task.Run(() => _userManager.Users.FirstOrDefault(u => u.Id == userId));
            
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult<Dictionary<CourseViewModel, float>>
                        .CreateFailure(message, 404);
                }

                Dictionary<CourseViewModel, float> dict = await Task.Run(() =>
                {
                    Dictionary<CourseViewModel, float> dict = new Dictionary<CourseViewModel, float>();

                    IEnumerable<UserCourse> userCourses = user.UserCourses
                        .Where(uc => uc.Course.Name.Contains(searcStr));
                    
                    foreach (var userCourse in userCourses)
                    {
                        CourseViewModel courseShort = _mapper.Map<CourseViewModel>(userCourse.Course);
                        
                        dict.Add(courseShort, userCourse.Progress);
                    }

                    return dict;
                });
                
                return ServiceResult<Dictionary<CourseViewModel, float>>.CreateSuccessResult(dict);
            }
            catch (Exception e)
            {
                return ServiceResult<Dictionary<CourseViewModel, float>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<Dictionary<CourseViewModel, float>>> GetActiveCoursesAsync
            (int userId, string searcStr = "")
        {
            try
            {
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success)
                {
                    return ServiceResult<Dictionary<CourseViewModel, float>>.CreateFailure("Database error.");
                }

                User user = userResult.Result;

                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult<Dictionary<CourseViewModel, float>>.CreateFailure(message, 404);
                }
                
                Dictionary<CourseViewModel, float> dict = await Task.Run(() =>
                {
                    return user.UserCourses
                        .Where(uc => !uc.IsCompleted)
                        .Where(uc => uc.Course.Name.Contains(searcStr))
                        .ToDictionary(uc => _mapper.Map<CourseViewModel>(uc.Course), 
                            uc => uc.Progress);
                });

                return ServiceResult<Dictionary<CourseViewModel, float>>.CreateSuccessResult(dict);
            }
            catch (Exception e)
            {
                return ServiceResult<Dictionary<CourseViewModel, float>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<ActiveCourseViewModel>> GetActiveCourseAsync(int userId, int courseId)
        {
            try
            {
                var userResult = await _userRepository.FindAsync(userId);
                
                if (!userResult.Success)
                {
                    return ServiceResult<ActiveCourseViewModel>.CreateFailure("Database error.");
                }

                User user = userResult.Result;
                
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult<ActiveCourseViewModel>.CreateFailure(message, 404);
                }

                UserCourse userCourse = user.UserCourses.FirstOrDefault(uc => uc.CourseId == courseId);

                if (userCourse is null)
                {
                    string message = $"User with id {userId} doesn't have course with id {courseId}.";
                    
                    return ServiceResult<ActiveCourseViewModel>.CreateFailure(message, 404);
                }

                ICollection<MaterialViewModel> completedMaterials = _mapper.Map<ICollection<MaterialViewModel>>(user
                    .Materials
                    .Join(userCourse.Course.Materials,
                        um => um.Id,
                        cm => cm.Id,
                        (um, cm) => um));

                ICollection<MaterialViewModel> uncompletedMaterials = _mapper.Map<ICollection<MaterialViewModel>>(userCourse
                    .Course.Materials
                    .Where(m => !user.Materials
                        .Select(cm => cm.Id).Contains(m.Id)));

                ActiveCourseViewModel activeCourse = new ActiveCourseViewModel()
                {
                    Id = userCourse.Course.Id,
                    Name = userCourse.Course.Name,
                    Description = userCourse.Course.Description,
                    CompletedMaterials = completedMaterials,
                    UncompletedMaterials = uncompletedMaterials
                };

                return ServiceResult<ActiveCourseViewModel>.CreateSuccessResult(activeCourse);
            } catch (Exception e)
            {
                return ServiceResult<ActiveCourseViewModel>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCreatedCoursesAsync
            (int userId, string searcStr = "")
        {
            try
            {
                User user = await Task.Run(() => _userManager.Users.FirstOrDefault(u => u.Id == userId));
            
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult<IEnumerable<CourseViewModel>>
                        .CreateFailure(message, 404);
                }
                
                IEnumerable<CourseViewModel> courses = _mapper.Map<IEnumerable<CourseViewModel>>(
                    await Task.Run(() => user.CreatedCourses.Where(c => c.Name.Contains(searcStr))));
                
                return ServiceResult<IEnumerable<CourseViewModel>>.CreateSuccessResult(courses);
            }
            catch (Exception e)
            {
                return ServiceResult<IEnumerable<CourseViewModel>>.CreateFailure(e);
            }
        }

        public async Task<ServiceResult<IEnumerable<CourseViewModel>>> GetCompletedCoursesAsync
            (int userId, string searcStr = "")
        {
            try
            {
                User user = await Task.Run(() => _userManager.Users.FirstOrDefault(u => u.Id == userId));
            
                if (user is null)
                {
                    string message = $"User with id {userId} doesn't exist.";
                    
                    return ServiceResult<IEnumerable<CourseViewModel>>
                        .CreateFailure(message, 404);
                }
                
                IEnumerable<CourseViewModel> courses = await Task.Run(() =>
                {
                    return user.UserCourses
                        .Where(uc => uc.IsCompleted)
                        .Select(uc => _mapper.Map<CourseViewModel>(uc.Course))
                        .Where(c => c.Name.Contains(searcStr))
                        .ToList();
                });
                
                return ServiceResult<IEnumerable<CourseViewModel>>.CreateSuccessResult(courses);
            }
            catch (Exception e)
            {
                return ServiceResult<IEnumerable<CourseViewModel>>.CreateFailure(e);
            }
        }
    }
}