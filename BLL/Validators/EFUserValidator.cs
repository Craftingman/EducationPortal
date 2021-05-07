using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BLL
{
    public class EFUserValidator : UserValidator<User>
    {
        private readonly IConfiguration _configuration;

        public EFUserValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();
 
            if (!Regex.IsMatch(user.Email.ToLower(), _configuration["ValidationPatterns:UserEmail"])) 
            {
                errors.Add(new IdentityError
                {
                    Description = $"E-Mail '{user.Email}' является некорректным. Введите корректный E-Mail."
                });
            }

            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}