using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.MovieRequestDTO
{
    public class AssignGenreMovieRequestDTO
    {
        public int ID { get; set; }
        public List<int> GenresID { get; set; } = null!;
        public IGenresRepository GenresRepository { get; set; } = null!;
        public IMovieRepository MovieRepository { get; set; } = null!;

    }
}
