using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;
using System.Linq.Dynamic.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MinimalAPIMoviez.Repositories
{
    public class MovieRepository(IHttpContextAccessor httpContextAccessor, 
        ApplicationDbContext context, IMapper mapper, ILogger<MovieRepository> logger) : IMovieRepository
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

                .Include(m=> m.GenresMovies)
                    .ThenInclude(m=> m.Genres)

                .Include(m=> m.ActorsMovies.OrderBy(am=> am.Actor))
                    .ThenInclude(m=> m.Actor)
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
        public async Task Assign(int id, List<ActorMovie> actors)
        {
            for (int i = 1; i <= actors.Count; i++)
            {
                actors[i - 1].Order = i;
            }
            var movie =  await context.movies.Include(m=> m.ActorsMovies)
                .FirstOrDefaultAsync(m=> m.Id == id);
            if (movie == null)
            {
                throw new ArgumentException($"The movie with id {id} did not found...");
            }

            // update the existing collection of ActorsMovies using AutoMapper
            movie.ActorsMovies = mapper.Map(actors, movie.ActorsMovies);
            await context.SaveChangesAsync();
        }

        public async Task<List<Movie>> Filter(MoviesFilterDTO moviesFilterDTO)
        {
            var moviesQueryable = context.movies.AsQueryable();

            #region filter for Parameters (Title, InCinema, FutureReleases, GenreId)
            if (!string.IsNullOrEmpty(moviesFilterDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x=> x.Title.Contains(moviesFilterDTO.Title));
            }

            if (moviesFilterDTO.InCinema)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InCinema);
            }
            if (moviesFilterDTO.FutureReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if(moviesFilterDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable.
                        Where(x => x.GenresMovies.
                            Select(y => y.GenreId).
                            Contains(moviesFilterDTO.GenreId));
            }
            if (!string.IsNullOrEmpty(moviesFilterDTO.OrderByField))
            {
                var orderKind = moviesFilterDTO.OrderByAscending ? "ascending" : "descending";
                try
                {
                    moviesQueryable = moviesQueryable.OrderBy($"{moviesFilterDTO.OrderByField} {orderKind}");
                }
                catch (Exception ex)
                {

                    logger.LogError(ex.Message);
                }
            }
            #endregion

            await httpContextAccessor.HttpContext!.InsertPaginationInResponseHeader(moviesQueryable);

            var movies = await moviesQueryable.Pagination(moviesFilterDTO.PaginationDTO).ToListAsync();

            return movies;
        }
    }
}
