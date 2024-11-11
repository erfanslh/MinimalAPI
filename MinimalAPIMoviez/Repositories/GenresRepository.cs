using Microsoft.EntityFrameworkCore;
using MinimalAPIMoviez;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly ApplicationDbContext context;
        public GenresRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<int> Create(Genre genre)
        {
            context.Add(genre);
            await context.SaveChangesAsync();
            return genre.Id;
        }

        public async Task<List<Genre>> GetAll()
        {
            return await context.genres.OrderBy(g => g.Name).ToListAsync();
        }



        public async Task<Genre?> GetbyID(int id)
        {
            return await context.genres.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}



