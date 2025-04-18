using MinimalAPIMoviez.Entities;
using Error = MinimalAPIMoviez.Entities.Error;

namespace MinimalAPIMoviez.Repositories
{
    public interface IErrorRepository
    {
        Task Create(Error error);
    }
}