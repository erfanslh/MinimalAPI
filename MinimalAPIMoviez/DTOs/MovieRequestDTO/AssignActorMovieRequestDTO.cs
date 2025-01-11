using AutoMapper;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.MovieRequestDTO
{
    public class AssignActorMovieRequestDTO
    {
        public int ID { get; set; }
        public List<AssignActorMovieDTO> AssignActorMovies { get; set; } = null!;
        public IMovieRepository MovieRepository { get; set; } = null!;
        public IActorRepository ActorRepository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
        
    }
}
