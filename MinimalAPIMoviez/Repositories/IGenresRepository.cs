using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genre genre);
        Task<List<Genre>> GetAll();
        Task<Genre?> GetByID(int id);


    }
}
