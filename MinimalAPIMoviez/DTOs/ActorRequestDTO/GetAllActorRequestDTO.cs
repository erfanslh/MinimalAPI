using AutoMapper;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.ActorRequestDTO
{
    public class GetAllActorRequestDTO
    {
        public IActorRepository Repository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
        //public int Page { get; set; } = 1;
        //public int RecordsPerPage { get; set; } = 10;


    }
}
