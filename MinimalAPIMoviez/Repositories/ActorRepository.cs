using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public class ActorRepository(ApplicationDbContext context,
                                IHttpContextAccessor httpContextAccessor) : IActorRepository
    {
        private readonly ApplicationDbContext context = context;

        public async Task<List<Actor>>  GetAll(PaginationDTO pagination)
        {
            var queryable = context.actors.AsQueryable();
            await httpContextAccessor.HttpContext!.InsertPaginationInResponseHeader(queryable);
            return await queryable.OrderBy(a => a.Name).Pagination(pagination).ToListAsync();
        }
        public async Task<Actor?> GetByID(int id)
        {
            return await context.actors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<List<Actor>> GetByName(string name)
        {
            return await context.actors.Where(a=> a.Name.Contains(name)).OrderBy(a=> a.Name).ToListAsync();
        }
        public async Task<int> Create(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }
        public async Task<bool> Exists(int id)
        {
            return await context.actors.AnyAsync(a=> a.Id == id);
        }
        public async Task<List<int>> Exists(List<int> ids)
        {
            return await context.actors.Where(a => ids.Contains(a.Id)).Select(a=> a.Id).ToListAsync();
        }
        public async Task Update(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();
        }

        public async Task Delete (int id)
        {
             await context.actors.Where(a => a.Id == id).ExecuteDeleteAsync();
        }
    }
}
