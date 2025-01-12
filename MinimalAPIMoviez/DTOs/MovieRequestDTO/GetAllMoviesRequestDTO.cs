using AutoMapper;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.MovieRequestDTO
{
    public class GetAllMoviesRequestDTO
    {
        public IMovieRepository Repository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;


    }
}
