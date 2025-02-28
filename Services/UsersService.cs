using Microsoft.AspNetCore.Identity;

namespace MinimalAPIsWithASPNetEF.Services
{
    public class UsersService(IHttpContextAccessor httpCtxAccessor, UserManager<IdentityUser> usrMgr) : IUsersService
    {
        public async Task<IdentityUser?> GetUser()
        {
            var emailClaim = httpCtxAccessor.HttpContext!
                .User.Claims.Where(x => x.Type == "email").FirstOrDefault();
            if (emailClaim is null)
            {
                return null;
            }
            var email = emailClaim.Value;
            return await usrMgr.FindByEmailAsync(email);
        }
    }
}
