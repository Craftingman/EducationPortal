using System.Threading.Tasks;
using Core;
using Core.Entities;

namespace BLL.Abstractions
{
    public interface IUserService
    {
        public Task<ServiceResult> RegisterUserAsync(User user);

        public Task<ServiceResult<User>> SignInUserAsync(string email, string password);

        public Task<bool> HasRoleAsync(User user, string role);
    }
}