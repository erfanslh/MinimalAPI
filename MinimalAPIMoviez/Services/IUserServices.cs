using Microsoft.AspNetCore.Identity;

namespace MinimalAPIMoviez.Services
{
    public interface IUserServices
    {
        Task<IdentityUser?> GetUser();
    }
}