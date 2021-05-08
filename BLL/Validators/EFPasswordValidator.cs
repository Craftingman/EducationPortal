using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BLL
{
    public class EFPasswordValidator : IPasswordValidator<User>
    {
        private readonly IConfiguration _configuration;

        public EFPasswordValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();
 
            if (!Regex.IsMatch(password, _configuration["ValidationPatterns:User:Password"]))
            {
                errors.Add(new IdentityError
                {
                    Description = "Формат пароля некорректен. Введите пароль в корректном формате."
                });
            }
            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}