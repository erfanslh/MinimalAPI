using AutoMapper;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.ActorRequestDTO
{
    public class GetByNameActorRequestDTO
    {
        public string Name { get; set; } = null!;
        public IActorRepository Repository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
    }
}
