using Microsoft.AspNetCore.Http;
using Path = System.IO.Path;

namespace MinimalAPIMoviez.Services
{
    public class LocalFileStorage(IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment environment) : IFileStorage
    {
        public Task Delete(string? route, string container)
        {
            if (string.IsNullOrEmpty(route))
            {
                return Task.CompletedTask;
            }

            var fileName = Path.GetFileName(route);
            var webRootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
            var fileDirectory = Path.Combine(webRootPath, container, fileName);

            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }

            return Task.CompletedTask;
        }

        public async Task<string> Store(string container, IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            var webRootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(webRootPath);

            var folder = Path.Combine(webRootPath, container);
            Directory.CreateDirectory(folder);

            var route = Path.Combine(folder, fileName);

            await using (var stream = File.Create(route))
            {
                await file.CopyToAsync(stream);
            }

            var request = httpContextAccessor.HttpContext?.Request;
            if (request is null)
            {
                return $"/{container}/{fileName}";
            }

            return $"{request.Scheme}://{request.Host}/{container}/{fileName}";
        }
    }
}
