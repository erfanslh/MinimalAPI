using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.DTOs;
using System.Security.Cryptography.Xml;
using AutoMapper;

namespace MinimalAPIMoviez.EndPoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder routeGroupBuilder)
        {
            #region CRUD
            //Create
            routeGroupBuilder.MapPost("/", Insert);
            //Get All
            routeGroupBuilder.MapGet("/", GetAll).CacheOutput(x => x.Expire(TimeSpan.FromSeconds(15)).Tag("cache-genre"));
            //Get an Entity
            routeGroupBuilder.MapGet("/{id:int}", GetById);
            //Edit
            routeGroupBuilder.MapPut("/{id:int}", Update);
            //Delete
            //***   "/genre (in MapGroup added)  /{id:int}"
            routeGroupBuilder.MapDelete("/{id:int}", Delete);
            #endregion

            return routeGroupBuilder;
        }
        #region Methods for Lambda
        //We made all our CRUD process (Endpoints) more readable by groupping them into Methods
        //*****
        // In Methods all Results are converted to TypedResults due to better type safety and enhanced readability
        //*****
        static async Task<Ok<List<GenreDTO>>> GetAll(IGenresRepository repository, IMapper mapper)
        {
            var genre = await repository.GetAll();
            //Using Select() in Linq - GenreDTO include => {empty id and empty name}
            // and we insert "genre" information into empty GenreDTO
            var genreDTO = mapper.Map<List<GenreDTO>>(genre);
            return TypedResults.Ok(genreDTO);
        }
        //*****
        //Get by ID
        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(IGenresRepository repository, int ID
            , IMapper mapper)
        {

            var genre = await repository.GetbyID(ID);
            if (genre == null)
            {
                return TypedResults.NotFound();
            }
            var genreDTO = mapper.Map<GenreDTO>(genre);
            return TypedResults.Ok(genreDTO);
        }
        //*****

        //Create
        static async Task<Created<GenreDTO>> Insert(CreateGenreDTO createGenreDTO,
            IOutputCacheStore iCache,
            IGenresRepository repository,
            IMapper mapper)
        {
            var genre = mapper.Map<Genre>(createGenreDTO);
            var id = await repository.Create(genre);
            await iCache.EvictByTagAsync("cache-genre", default);

            var genreDTO = mapper.Map<GenreDTO>(genre);

            return TypedResults.Created($"/genre/{id}", genreDTO);
        }
        //******

        //Update
        static async Task<Results<NotFound, NoContent>> Update(int ID,
            CreateGenreDTO createGenreDTO,
            IGenresRepository repository,
            IOutputCacheStore cacheStore,
            IMapper mapper)
        {
            //*** here we should use "await" ==> cuz it has Async and we are working with DB
            var Exists = await repository.Exist(ID);
            if (!Exists)
            {
                return TypedResults.NotFound();
            }
            var genre = mapper.Map<Genre>(createGenreDTO);
            //We need ID to update, which is not included in our DTO "createGenreDTO"
            genre.Id = ID; 
          //*****************
            await repository.Update(genre);
            // we Use IOutputCacheStore to cleanup the Caches by creating an Object of it
            // ("IOutputCacheStore cacheStore"), then we use the object to implement cleanup Cache
            await cacheStore.EvictByTagAsync("cache-genre", default);
            //cuz we dont return anything in Update so we use
            //    "Results.NoContent()"
            return TypedResults.NoContent();
        }
        //Delete
        static async Task<Results<NotFound, NoContent>> Delete(IGenresRepository repository, int ID
            , IOutputCacheStore cacheStore)
        {
            var existing = await repository.Exist(ID);
            if (!existing)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(ID);
            await cacheStore.EvictByTagAsync("cache-genre", default);
            return TypedResults.NoContent();
        }
        #endregion
    }
}
