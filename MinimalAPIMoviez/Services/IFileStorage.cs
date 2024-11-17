using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MinimalAPIMoviez.Services
{
    public interface IFileStorage
    {
        Task<string> Store(string container, IFormFile file);
        Task Delete(string? route, string container);

        async Task<string> UpdateImage(string container, IFormFile file, string? route)
        {
            await Delete(route, container);
            return await Store(container, file);
        }
    }
}
