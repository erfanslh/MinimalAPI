using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;

namespace MinimalAPIMoviez.DTOs.ActorRequestDTO
{
    public class UpdateActorRequestDTO
    {
        public int ID { get; set; }
        public IMapper Mapper { get; set; } = null!;
        public IActorRepository Repository { get; set; } = null!;
        public IOutputCacheStore CacheStore { get; set; } = null!;
        public IFileStorage FileStorage { get; set; } = null!;
    }
}
