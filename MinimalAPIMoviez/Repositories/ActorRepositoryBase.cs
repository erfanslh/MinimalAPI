using Microsoft.EntityFrameworkCore;
using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public class ActorRepositoryBase
    {
        private readonly ApplicationDbContext context;
        public async Task<int> Create(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task Delete(int id)
        {
            await context.actor.Where(a => a.Id == id).ExecuteDeleteAsync();
        }
        public async Task<bool> Exists(int id)
        {
            return await context.actor.AnyAsync(a => a.Id == id);
        }


        public async Task<List<Actor>> GetAll()
        {
            return await context.actor.OrderBy(a => a.Name).ToListAsync();
        }
        public async Task<Actor?> GetByID(int id)
        {
            return await context.actor.FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task Update(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();
        }
    }
}