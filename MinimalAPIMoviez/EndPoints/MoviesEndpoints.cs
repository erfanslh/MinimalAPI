using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Filters;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;

namespace MinimalAPIMoviez.EndPoints
{
    public static class MoviesEndpoints
    {
        private readonly static string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder routeGroup)
        {
            routeGroup.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDTO>>()
                .RequireAuthorization("isadmin");
            routeGroup.MapPost("/{id:int}/assignGenre",AssignGenres).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDTO>>()
                .RequireAuthorization("isadmin");
            routeGroup.MapPost("/{id:int}/assignActor", AssignActor).RequireAuthorization("isadmin");
            routeGroup.MapPut("/", Update).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDTO>>()
                .RequireAuthorization("isadmin");
            routeGroup.MapDelete("/", Delete);
            routeGroup.MapGet("/", GetAllMovies)
                                        .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("movie-get"));
            routeGroup.MapGet("/{id:int}", GetByID);
            return routeGroup;
        }
        static async Task<IResult> Delete(int id,IMovieRepository movieRepository,
            IOutputCacheStore cache)
        {
            var findMovie = await movieRepository.Exists(id);
            if (findMovie == false)
            {
                return TypedResults.BadRequest("Movie does not exist");
            }
            await movieRepository.Delete(id);
            await cache.EvictByTagAsync("movie-get", default);
            return TypedResults.NoContent();

        }
        static async Task<IResult> Update(int id, CreateMovieDTO createMovieDTO,
            IMovieRepository movieRepository, 
            IOutputCacheStore cache,
            IMapper mapper)
        {
            var findMovie = await movieRepository.Exists(id);
            if (findMovie == false)
            {
                return TypedResults.BadRequest("the movie does not exist");
            }
            var MapMovie = mapper.Map<Movie>(createMovieDTO);
            await movieRepository.Update(MapMovie);
            await cache.EvictByTagAsync("movie-get", default);
            return TypedResults.NoContent();
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
        public static async Task<IResult> AssignActor(int id, List<AssignActorMovieDTO> assignActorMovies,
            IActorRepository actorRepository, IMovieRepository movieRepository, IMapper mapper)
        {
            if  (!await actorRepository.Exists(id))
            {
                return TypedResults.NotFound();
            }

            var existingActorsIds = new List<int>();
            // Extract the actor IDs from the list of AssignActorMovieDTO objects
            var actorsIds = assignActorMovies.Select(a=> a.ActorId).ToList();

            if (assignActorMovies.Count !=0 )
            {
                existingActorsIds = await actorRepository.Exists(actorsIds);
            }

         // if the number of existing actor IDs does not match the number of provided actor IDs,
            if (existingActorsIds.Count != assignActorMovies.Count )
            {
                // Find the IDs of actors that do not exist
                var nonExistingActors = actorsIds.Except(existingActorsIds);
                var nonExistingActorsCSV = string.Join (", ", nonExistingActors);
                return TypedResults.BadRequest($"Actor with the ID:{nonExistingActorsCSV} did not found");
            }
            var actor = mapper.Map<List<ActorMovie>>(assignActorMovies);
            await movieRepository.Assign(id, actor);
            return TypedResults.NoContent();
        }


    }
}
