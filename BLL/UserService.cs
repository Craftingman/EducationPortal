using System;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions;
using Core;
using Core.Entities;
using EFCore;
using Microsoft.AspNetCore.Identity;

namespace BLL
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserService(
            UserManager<User> userManager, 
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ServiceResult> RegisterUserAsync(User user)
        {
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return ServiceResult.CreateSuccessResult();
            }

            string errorString = String.Concat(result.Errors.Select(e => e.ToString()));
            int.TryParse(result.Errors.FirstOrDefault()?.Code, out int code);
            
            return ServiceResult.CreateFailure(errorString, code);
        }

        public async Task<ServiceResult<User>> SignInUserAsync(string email, string password)
        {
            User user = await Task.Run(() => _userManager.Users.FirstOrDefault(u => u.Email == email));

            if (user == null)
            {
                return ServiceResult<User>.CreateFailure("Bad Credentials", 401);
            }
            
            var result = _userManager.PasswordHasher.VerifyHashedPassword(
                user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
            {
                return ServiceResult<User>.CreateSuccessResult(user);
            }
            
            return ServiceResult<User>.CreateFailure("Bad Credentials", 401);
        }

        public async Task<bool> HasRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }
    }
}