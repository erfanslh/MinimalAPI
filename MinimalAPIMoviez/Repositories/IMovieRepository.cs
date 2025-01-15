using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public interface IMovieRepository
    {
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<List<Movie>> GetAll(PaginationDTO pagination);
        Task<Movie?> GetByID(int id);
        Task<bool> Exists(int id);
        Task Update(Movie movie);
        Task Assign(int id, List<int> ids);
        Task Assign(int id, List<ActorMovie> actors);
        Task<List<Movie>> Filter(MoviesFilterDTO moviesFilterDTO);
    }
}