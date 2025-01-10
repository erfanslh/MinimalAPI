using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.GenreRequestDTO
{
    public class DeleteGenresRequestDTO
    {
        public int ID { get; set; }
        public IGenresRepository Repository { get; set; } = null!;
        public IOutputCacheStore CacheStore { get; set; } = null!;
    }
}
