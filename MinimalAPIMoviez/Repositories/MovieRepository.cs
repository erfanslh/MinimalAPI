using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public class MovieRepository(IHttpContextAccessor httpContextAccessor, 
        ApplicationDbContext context, IMapper mapper) : IMovieRepository
    {
        public async Task<List<Movie>> GetAll(PaginationDTO pagination)
        {
            var queryable = context.movies.AsQueryable();
            await httpContextAccessor.HttpContext!.InsertPaginationInResponseHeader(queryable);
            return await queryable.OrderBy(m => m.Title).Pagination(pagination).ToListAsync();
        }

        public async Task<Movie?> GetByID(int id)
        {
            return await context.movies
                .Include(m=> m.commentsfk)
                .AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

        }
        public async Task<bool> Exists(int id)
        {
            return await context.movies.AnyAsync(m => m.Id == id);
        }
        public async Task<int> Create(Movie movie)
        {
            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;
        }

        public async Task Update(Movie movie)
        {
            context.movies.Update(movie);
            await context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await context.movies.Where(m => m.Id == id).ExecuteDeleteAsync();
        }
        public async Task Assign (int id , List<int> ids)
        {
            var movie = await context.movies.Include(m=> m.GenresMovies).FirstOrDefaultAsync(m=> m.Id == id);
            if (movie == null) 
            {
                throw new ArgumentException($"movie with id:{id} is not found");
            }
            // Creating an IEnumerable value to fill each genreID
            var genresMovie = ids.Select(genreid => new GenreMovie { GenreId = genreid });
            // add-keep-delete
            movie.GenresMovies = mapper.Map(genresMovie, movie.GenresMovies);
            await context.SaveChangesAsync();
        }
    }
}
