using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public interface IActorRepository
    {
        Task<List<Actor>> GetAll(PaginationDTO pagination);
        Task<Actor?> GetByID(int id);
        Task<int> Create(Actor actor);
        Task<bool> Exists(int id);
        Task Update(Actor actor);
        Task Delete(int id);
        Task<List<Actor>> GetByName(string name);
        Task<List<int>> Exists(List<int> ids);
        Task<bool> ActorExists(string name, DateTime birthDayDate);
    }
}
