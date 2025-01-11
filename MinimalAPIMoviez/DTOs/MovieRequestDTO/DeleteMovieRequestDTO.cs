using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.MovieRequestDTO
{
    public class DeleteMovieRequestDTO
    {
        public int ID { get; set; }
        public IMovieRepository Repository { get; set; } = null!;
        public IOutputCacheStore CacheStore { get; set; } = null!;
    }
}
