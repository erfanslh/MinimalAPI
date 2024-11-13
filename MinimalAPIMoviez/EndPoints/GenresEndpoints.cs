using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;

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
        static async Task<Ok<List<Genre>>> GetAll(IGenresRepository repository)
        {
            var allInList = await repository.GetAll();
            return TypedResults.Ok(allInList);
        }
        //*****
        //Get by ID
        static async Task<Results<Ok<Genre>, NotFound>> GetById(IGenresRepository repository, int ID)
        {
            var genreId = await repository.GetbyID(ID);
            if (genreId == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(genreId);
        }
        //*****

        //Create
        static async Task<Created<Genre>> Insert(Genre genre, IOutputCacheStore iCache, IGenresRepository repository)
        {
            var id = await repository.Create(genre);
            await iCache.EvictByTagAsync("cache-genre", default);
            return TypedResults.Created($"/genre/{id}", genre);
        }
        //******

        //Update
        static async Task<Results<NotFound, NoContent>> Update(int ID, Genre genre, IGenresRepository repository
            , IOutputCacheStore cacheStore)
        {
            //*** here we should use "await" ==> cuz it has Async and we are working with DB
            var Exists = await repository.Exist(ID);
            if (!Exists)
            {
                return TypedResults.NotFound();
            }
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
