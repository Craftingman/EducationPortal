using System;
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

        //private readonly IRepositoryBase<User> _userRepository;

        private readonly IUserValidator<User> _userValidator;
        
        private readonly IPasswordValidator<User> _passwordValidator;

        public UserService(
            UserManager<User> userManager, 
            RoleManager<IdentityRole<int>> roleManager,
            //IRepositoryBase<User> userRepository,
            IUserValidator<User> userValidator,
            IPasswordValidator<User> passwordValidator,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            //_userRepository = userRepository;
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

        public async Task<ServiceResult<bool>> UserExists(string email)
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

        public async Task<ServiceResult<Dictionary<CourseViewModel, float>>> GetInProgressCoursesAsync
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
                    return user.UserCourses
                        .Where(uc => uc.Progress != 1)
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
                        .Where(uc => uc.Progress == 1)
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