using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.DTOs;
using System.Security.Cryptography.Xml;
using AutoMapper;
using FluentValidation;
using MinimalAPIMoviez.Filters;
using MinimalAPIMoviez.DTOs.GenreRequestDTO;

namespace MinimalAPIMoviez.EndPoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder routeGroupBuilder)
        {
            #region CRUD
            //Create
            routeGroupBuilder.MapPost("/", Insert).AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                .RequireAuthorization("isadmin");
            //Get All
            routeGroupBuilder.MapGet("/", GetAll).CacheOutput(x => x.Expire(TimeSpan.FromSeconds(15)).Tag("cache-genre"));
            //Get an Entity
            routeGroupBuilder.MapGet("/{id:int}", GetById).RequireAuthorization();
            //Edit
            routeGroupBuilder.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                .RequireAuthorization("isadmin");
            //Delete
            //***   "/genre (in MapGroup added)  /{id:int}"
            routeGroupBuilder.MapDelete("/{id:int}", Delete).RequireAuthorization("isadmin");
            #endregion

            return routeGroupBuilder;
        }

        //We made all our CRUD process (Endpoints) more readable by groupping them into Methods
        //*****
        // In Methods all Results are converted to TypedResults due to better type safety and enhanced readability
        //*****
        #region GetAll
        static async Task<Ok<List<GenreDTO>>> GetAll([AsParameters] GetAllGenresRequestDTO model)
        {
            var genre = await model.Repository.GetAll();
            //Using Select() in Linq - GenreDTO include => {empty id and empty name}
            // and we insert "genre" information into empty GenreDTO
            var genreDTO = model.Mapper.Map<List<GenreDTO>>(genre);
            return TypedResults.Ok(genreDTO);
        }
        #endregion

        #region Get_by_Id
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
        #endregion

        #region Create
        static async Task<Created<GenreDTO>> Insert(CreateGenreDTO createGenreDTO, [AsParameters] InsertGenresRequestDTO model)
           
        {
            var genre = model.Mapper.Map<Genre>(createGenreDTO);
            var id = await model.Repository.Create(genre);
            await model.CacheStore.EvictByTagAsync("cache-genre", default);

            var genreDTO = model.Mapper.Map<GenreDTO>(genre);

            return TypedResults.Created($"/genre/{id}", genreDTO);
        }
        #endregion

        #region Update
        static async Task<IResult> Update(CreateGenreDTO createGenreDTO, [AsParameters] UpdateGenresRequestDTO model)
        {
            //*** here we should use "await" ==> cuz it has Async and we are working with DB
            var Exists = await model.Repository.Exist(model.ID);
            if (!Exists)
            {
                return TypedResults.NotFound();
            }
            var genre = model.Mapper.Map<Genre>(createGenreDTO);
            //We need ID to update, which is not included in our DTO "createGenreDTO"
            genre.Id = model.ID; 
          //*****************
            await model.Repository.Update(genre);
            // we Use IOutputCacheStore to cleanup the Caches by creating an Object of it
            // ("IOutputCacheStore cacheStore"), then we use the object to implement cleanup Cache
            await model.CacheStore.EvictByTagAsync("cache-genre", default);
            //cuz we dont return anything in Update so we use
            //    "Results.NoContent()"
            return TypedResults.NoContent();
        }
        #endregion

        #region Delete
        //Delete
        static async Task<Results<NotFound, NoContent>> Delete([AsParameters] DeleteGenresRequestDTO model)
        {
            var existing = await model.Repository.Exist(model.ID);
            if (!existing)
            {
                return TypedResults.NotFound();
            }

            await model.Repository.Delete(model.ID);
            await model.CacheStore.EvictByTagAsync("cache-genre", default);
            return TypedResults.NoContent();
        }
        #endregion

    }
}
