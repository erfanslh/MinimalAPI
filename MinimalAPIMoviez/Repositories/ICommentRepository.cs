using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public interface ICommentRepository
    {
        Task<int> Create(Comment cmt);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<List<Comment>> GetAll(int movieID);
        Task<Comment?> GetByID(int id);
        Task Update(Comment cmt);
    }
}