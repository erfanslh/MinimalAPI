using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;

namespace MinimalAPIMoviez.DTOs.MovieRequestDTO
{
    public class CreateMovieRequestDTO
    {
        public IMovieRepository Repository { get; set; } = null!;
        public IFileStorage FileStorage { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
        public IOutputCacheStore CacheStore { get; set; } = null!;
    }
}
