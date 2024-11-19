using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public interface IActorRepository
    {
        Task<List<Actor>> GetAll();
        Task<Actor?> GetByID(int id);
        Task<int> Create(Actor actor);
        Task<bool> Exists(int id);
        Task Update(Actor actor);
        Task Delete(int id);
        Task<List<Actor>> GetByName(string name);
    }
}
