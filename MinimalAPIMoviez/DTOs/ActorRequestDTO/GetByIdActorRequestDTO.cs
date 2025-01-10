using AutoMapper;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.ActorRequestDTO
{
    public class GetByIdActorRequestDTO
    {
        public int ID { get; set; }
        public IActorRepository Repository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;

    }
}
