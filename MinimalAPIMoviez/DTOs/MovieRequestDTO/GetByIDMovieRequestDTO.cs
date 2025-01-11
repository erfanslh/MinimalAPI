using AutoMapper;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.MovieRequestDTO
{
    public class GetByIDMovieRequestDTO
    {
        public int ID { get; set; }
        public IMovieRepository Repository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
    }
}
