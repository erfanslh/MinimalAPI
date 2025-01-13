using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.DTOs.MovieRequestDTO;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Filters;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;
using MinimalAPIMoviez.Utilities;

namespace MinimalAPIMoviez.EndPoints
{
    public static class MoviesEndpoints
    {
        private readonly static string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder routeGroup)
        {
            routeGroup.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDTO>>()
                .RequireAuthorization("isadmin").WithOpenApi(options =>
                {
                    options.Summary = "Create a Movie";
                    options.Description = "Here you can add a Movie, You need to be Authorized as an ADMIN first.";
                    return options;

                }); 
            routeGroup.MapPost("/{id:int}/assignGenre",AssignGenres).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDTO>>()
                .RequireAuthorization("isadmin").WithOpenApi(options =>
                {
                    options.Summary = "Assign a Genre to a Movie";
                    options.Description = "Enter the ID of the Movie you want to be Assigned to the Genre";
                    options.Parameters[0].Description = "ID of the Movie you want to be assigned";
                    return options;

                }); 
            routeGroup.MapPost("/{id:int}/assignActor", AssignActor).RequireAuthorization("isadmin").WithOpenApi(options =>
            {
                options.Summary = "Assign an Actor to a Movie";
                options.Description = "You need the Movie-ID and Actor-ID, You must be Authorized as an ADMIN first";
                options.Parameters[0].Description = "ID of the Movie you want to be assigned to the Actor";
                return options;

            }); 

            routeGroup.MapPut("/", Update).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDTO>>()
                .RequireAuthorization("isadmin").WithOpenApi(options =>
                {
                    options.Summary = "Update a Movie";
                    options.Description = "Enter the ID of the Movie you want to Update, You need to be Authorized as an ADMIN";
                    options.Parameters[0].Description = "ID of the Movie you want to Update";
                    return options;

                }); 

            routeGroup.MapDelete("/", Delete).WithOpenApi(options => 
                    {
                        options.Summary = "Delete a Movie";
                        options.Description = "Enter the ID of the Movie you want to Delete";
                        options.Parameters[0].Description = "ID of the Movie you want to Delete";
                        return options;

                    });

            routeGroup.MapGet("/", GetAllMovies).CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1))
            .Tag("movie-get")).AddPaginationParameters().WithOpenApi(options => {
                options.Summary = "Get all Movies";
                options.Description = "Click on Execute to receive all stored Movies in the Database";
                options.Parameters[0].Description = "Page Number";
                options.Parameters[1].Description = "Count of Movies per page";
                return options;
            });

            routeGroup.MapGet("/{id:int}", GetByID).WithOpenApi(options =>
            {
                options.Summary = "Get a Movie";
                options.Description = "Enter the ID of the Movie you want to view it's Details";
                options.Parameters[0].Description = "ID of the Movie";
                return options;

            });
            return routeGroup;
        }
        #region Delete
        static async Task<IResult> Delete([AsParameters] DeleteMovieRequestDTO model)
        {
            var findMovie = await model.Repository.Exists(model.ID);
            if (findMovie == false)
            {
                return TypedResults.BadRequest("Movie does not exist");
            }
            await model.Repository.Delete(model.ID);
            await model.CacheStore.EvictByTagAsync("movie-get", default);
            return TypedResults.NoContent();

        }
        #endregion

        #region Update
        static async Task<IResult> Update(CreateMovieDTO createMovieDTO, [AsParameters] UpdateMovieRequestDTO model)
        {
            var findMovie = await model.Repository.Exists(model.ID);
            if (findMovie == false)
            {
                return TypedResults.BadRequest("the movie does not exist");
            }
            var MapMovie = model.Mapper.Map<Movie>(createMovieDTO);
            await model.Repository.Update(MapMovie);
            await model.CacheStore.EvictByTagAsync("movie-get", default);
            return TypedResults.NoContent();
        }
        #endregion

        #region Create
        public static async Task<Created<MovieDTO>> Create([FromForm] CreateMovieDTO createMovieDTO, [AsParameters] CreateMovieRequestDTO model)
        {
            var movie = model.Mapper.Map<Movie>(createMovieDTO);
            if (createMovieDTO.CoverImage != null)
            {
                var url = await model.FileStorage.Store(container, createMovieDTO.CoverImage);
                movie.CoverImage = url;
            }

            var createRep= await model.Repository.Create(movie);
            var movieDTO = model.Mapper.Map<MovieDTO>(movie);
            await model.CacheStore.EvictByTagAsync("movie-get", default);
            return TypedResults.Created($"/{createRep}", movieDTO);
        }
        #endregion

        #region GetAllMovies
        public static async Task<Ok<List<MovieDTO>>> GetAllMovies([AsParameters] GetAllMoviesRequestDTO model, PaginationDTO paginationDTO)
        {
            var getAll = await model.Repository.GetAll(paginationDTO);
            var mapping = model.Mapper.Map<List<MovieDTO>>(getAll);
            return TypedResults.Ok(mapping);
        }
        #endregion

        #region GetByID
        public static async Task<Results<Ok<MovieDTO>,NotFound>> GetByID([AsParameters] GetByIDMovieRequestDTO model)
        {
            var movie = await model.Repository.GetByID(model.ID);
            if (movie is null)
            {
                return TypedResults.NotFound();
            }
            var map = model.Mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(map);
        }
        #endregion

        #region Assign_Genre_to_Movie
        public static async Task<IResult> AssignGenres([AsParameters] AssignGenreMovieRequestDTO model)
        {
            if (!await model.MovieRepository.Exists(model.ID))
            {
                return TypedResults.NotFound();
            }
            var existingGenre = new List<int>();

            if (model.GenresID.Count != 0)
            {
                existingGenre = await model.GenresRepository.Exists(model.GenresID);
            }
            
            if (model.GenresID.Count != existingGenre.Count)
            {
                var nonExistingGenre = model.GenresID.Except(existingGenre);
                var nonExistingGenreCSV = string.Join(", ", nonExistingGenre);
                return TypedResults.BadRequest($"The genre with id:{nonExistingGenreCSV} did not found");
            }
            await model.MovieRepository.Assign(model.ID, existingGenre);
            return TypedResults.NoContent();
        }
        #endregion

        #region Assign_Actor_To_Movie
        public static async Task<IResult> AssignActor([AsParameters] AssignActorMovieRequestDTO model)
        {
            if  (!await model.ActorRepository.Exists(model.ID))
            {
                return TypedResults.NotFound();
            }

            var existingActorsIds = new List<int>();
            // Extract the actor IDs from the list of AssignActorMovieDTO objects
            var actorsIds = model.AssignActorMovies.Select(a=> a.ActorId).ToList();

            if (model.AssignActorMovies.Count !=0 )
            {
                existingActorsIds = await model.ActorRepository.Exists(actorsIds);
            }

         // if the number of existing actor IDs does not match the number of provided actor IDs,
            if (existingActorsIds.Count != model.AssignActorMovies.Count )
            {
                // Find the IDs of actors that do not exist
                var nonExistingActors = actorsIds.Except(existingActorsIds);
                var nonExistingActorsCSV = string.Join (", ", nonExistingActors);
                return TypedResults.BadRequest($"Actor with the ID:{nonExistingActorsCSV} did not found");
            }
            var actor = model.Mapper.Map<List<ActorMovie>>(model.AssignActorMovies);
            await model.MovieRepository.Assign(model.ID, actor);
            return TypedResults.NoContent();
        }
        #endregion


    }
}
