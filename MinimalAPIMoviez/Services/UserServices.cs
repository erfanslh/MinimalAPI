using Microsoft.AspNetCore.Identity;

namespace MinimalAPIMoviez.Services
{
    public class UserServices(IHttpContextAccessor httpContextAccessor,
        UserManager<IdentityUser> userManager) : IUserServices
    {
        public async Task<IdentityUser?> GetUser()
        {
            var emailClaim = httpContextAccessor.HttpContext!
                .User.Claims.Where(x => x.Type == "email").FirstOrDefault();
            if (emailClaim is null)
            {
                return null;
            }
            var email = emailClaim.Value;
            return await userManager.FindByEmailAsync(email);
        }
    }
}
