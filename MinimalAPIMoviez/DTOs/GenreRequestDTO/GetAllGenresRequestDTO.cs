using AutoMapper;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.GenreRequestDTO
{
    public class GetAllGenresRequestDTO
    {
        public IGenresRepository Repository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
    }
}
