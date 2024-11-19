using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public class ActorRepository : IActorRepository
    {
        private readonly ApplicationDbContext context;
        public ActorRepository(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task<List<Actor>>  GetAll()
        {
            return await context.actor.OrderBy(a => a.Name).ToListAsync();
        }
        public async Task<Actor?> GetByID(int id)
        {
            return await context.actor.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<List<Actor>> GetByName(string name)
        {
            return await context.actor.Where(a=> a.Name.Contains(name)).OrderBy(a=> a.Name).ToListAsync();
        }
        public async Task<int> Create(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }
        public async Task<bool> Exists(int id)
        {
            return await context.actor.AnyAsync(a=> a.Id == id);
        }
        public async Task Update(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();
        }

        public async Task Delete (int id)
        {
             await context.actor.Where(a => a.Id == id).ExecuteDeleteAsync();
        }
    }
}
