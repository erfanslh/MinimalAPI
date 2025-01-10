using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;

namespace MinimalAPIMoviez.DTOs.ActorRequestDTO
{
    public class DeleteActorRequestDTO
    {
        public int ID { get; set; }
        public IActorRepository Repository { get; set; } = null!;
        public IOutputCacheStore CacheStore { get; set; } = null!;
        public IFileStorage FileStorage { get; set; } = null!;
    }
}
