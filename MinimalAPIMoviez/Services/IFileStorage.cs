using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;

namespace MinimalAPIMoviez.Services
{
    public interface IFileStorage
    {
        Task<string> Store(string container, IFormFile file);
        Task Delete(string? route, string container);

        async Task<string> Update(string? route,  string container, IFormFile file)
        {
            await Delete(route, container);
            return await Store(container, file);
        }
    }
}
