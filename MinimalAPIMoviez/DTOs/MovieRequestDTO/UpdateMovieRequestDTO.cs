using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.MovieRequestDTO
{
    public class UpdateMovieRequestDTO
    {
        public int ID { get; set; }
        public IMovieRepository Repository { get; set; } = null!;
        public IOutputCacheStore CacheStore { get; set; } = null!;
        public IMapper Mapper{ get; set; } = null!;
    }
}
