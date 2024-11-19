
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System;

namespace MinimalAPIMoviez.Services
{
    public class LocalFileStorage(HttpContextAccessor httpContextAccessor,
        IWebHostEnvironment environment) : IFileStorage
    {
        public Task Delete(string? route, string container)
        {
            if (string.IsNullOrEmpty(route))
            {
                return Task.CompletedTask;
            }
            var fileName = Path.GetFileName(route);
            var fileDirectory = Path.Combine(environment.WebRootPath, container, fileName);

            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
            return Task.CompletedTask;
        }  
            public async Task<string> Store(string container, IFormFile file)
        {
            var getExtension = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid()}{getExtension}";

            // Combine the web root path with the container name to get the full directory path.
            string folder = Path.Combine(environment.WebRootPath, container);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            // Combine the directory path with the unique file name to get the full file path.
            string route = Path.Combine(folder, fileName);

            // Temporarily store the uploaded file's data using MemoryStream
            using (var ms = new MemoryStream())
            {
                // Copy the uploaded file into the memory stream.
                await file.CopyToAsync(ms);
                // Convert the memory stream's content into an array.
                var content = ms.ToArray();
                // Write the byte array to the target file path on the server.
                await File.WriteAllBytesAsync(route, content);
            };
            // Get Schema + Host and combine them inot a URL
            var schema = httpContextAccessor.HttpContext!.Request.Scheme;
            var host = httpContextAccessor.HttpContext!.Request.Host;
            var url = $"{schema}:\\{host}";

            // Combine the base URL, container name, and file name to create the public file URL.
            var urlFile = Path.Combine(url, container, fileName).Replace("\\", "/");

            return urlFile;
        }
    }
}



