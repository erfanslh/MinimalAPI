using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.GenreRequestDTO
{
    public class UpdateGenresRequestDTO
    {
        public int ID { get; set; }
        public IGenresRepository Repository { get; set; } = null!;
        public IOutputCacheStore CacheStore { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
    }
}
