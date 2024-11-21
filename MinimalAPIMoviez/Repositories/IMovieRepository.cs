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
        Task Update(Movie movie);
    }
}