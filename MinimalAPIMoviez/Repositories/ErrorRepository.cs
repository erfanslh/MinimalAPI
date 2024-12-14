using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public class ErrorRepository(ApplicationDbContext context) : IErrorRepository
    {
        public async Task Create(Error error)
        {
            context.Add(error);
            await context.SaveChangesAsync();
        }
    }
}
