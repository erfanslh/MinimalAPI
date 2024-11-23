using Microsoft.EntityFrameworkCore;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public class CommentRepository(ApplicationDbContext context) : ICommentRepository
    {

        public async Task<List<Comment>> GetAll(int movieID)
        {
            return await context.comments.Where(c => c.MovieId == movieID).ToListAsync();
        }
        public async Task<Comment?> GetByID(int id)
        {
            return await context.comments.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<int> Create(Comment cmt)
        {
            context.Add(cmt);
            await context.SaveChangesAsync();
            return cmt.Id;
        }
        public async Task<bool> Exists(int id)
        {
            return await context.comments.AnyAsync(c => c.Id == id);
        }
        public async Task Update(Comment cmt)
        {
            context.Update(cmt);
            await context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            await context.comments.Where(c => c.Id == id).ExecuteDeleteAsync();
        }
    }
}
