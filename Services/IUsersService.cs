using Microsoft.AspNetCore.Identity;

namespace MinimalAPIsWithASPNetEF.Services
{
    public interface IUsersService
    {
        Task<IdentityUser?> GetUser();
    }
}