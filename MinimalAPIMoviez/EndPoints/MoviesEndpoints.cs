using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;

namespace MinimalAPIMoviez.EndPoints
{
    public static class MoviesEndpoints
    {
        private readonly static string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder routeGroup)
        {
            routeGroup.MapPost("/", Create).DisableAntiforgery();
            routeGroup.MapPost("/{id:int}/assignGenre",AssignGenres).DisableAntiforgery();
            routeGroup.MapGet("/", GetAllMovies)
                                        .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("movie-get"));
            routeGroup.MapGet("/{id:int}", GetByID);
            return routeGroup;
        }

        public static async Task<Created<MovieDTO>> Create([FromForm] CreateMovieDTO createMovieDTO, 
                IMovieRepository repository, IFileStorage fileStorage, IMapper mapper,IOutputCacheStore cache )
        {
            var movie = mapper.Map<Movie>(createMovieDTO);
            if (createMovieDTO.CoverImage != null)
            {
                var url = await fileStorage.Store(container, createMovieDTO.CoverImage);
                movie.CoverImage = url;
            }

            var createRep= await repository.Create(movie);
            var movieDTO = mapper.Map<MovieDTO>(movie);
            await cache.EvictByTagAsync("movie-get", default);
            return TypedResults.Created($"/{createRep}", movieDTO);
        }

        public static async Task<Ok<List<MovieDTO>>> GetAllMovies(IMovieRepository repository,IMapper mapper,
                int page = 1, int recordsperpage = 10)
        {
            var pagination = new PaginationDTO { Page=page,RecordsPerPage = recordsperpage };
            var getAll = await repository.GetAll(pagination);
            var mapping = mapper.Map<List<MovieDTO>>(getAll);
            return TypedResults.Ok(mapping);
        }
        public static async Task<Results<Ok<MovieDTO>,NotFound>> GetByID(int id, IMovieRepository repository, IMapper mapper)
        {
            var movie = await repository.GetByID(id);
            if (movie is null)
            {
                return TypedResults.NotFound();
            }
            var map = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(map);
        }
        public static async Task<IResult> AssignGenres(int id, List<int>genresID,
            IMovieRepository movieRepository, IGenresRepository genresRepository)
        {
            if (!await movieRepository.Exists(id))
            {
                return TypedResults.NotFound();
            }
            var existingGenre = new List<int>();

            if (genresID.Count != 0)
            {
                existingGenre = await genresRepository.Exists(genresID);
            }
            
            if (genresID.Count != existingGenre.Count)
            {
                var nonExistingGenre = genresID.Except(existingGenre);
                var nonExistingGenreCSV = string.Join(", ", nonExistingGenre);
                return TypedResults.BadRequest($"The genre with id:{nonExistingGenreCSV} did not found");
            }
            await movieRepository.Assign(id, existingGenre);
            return TypedResults.NoContent();
        }
    }
}
