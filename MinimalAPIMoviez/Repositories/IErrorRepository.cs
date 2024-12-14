using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public interface IErrorRepository
    {
        Task Create(Error error);
    }
}