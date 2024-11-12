using FluentAssertions.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPIMoviez;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;
using System.Collections.Generic;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Service Zone - Start

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(config =>
            {
                config.WithOrigins(builder.Configuration["AllowedOrigin"]!).AllowAnyMethod().AllowAnyHeader();
            });

            options.AddPolicy("free", configuration =>
            {
                configuration.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
        builder.Services.AddOutputCache();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IGenresRepository, GenresRepository>();

        //Service Zone - End

        var app = builder.Build();

        // Middleware zone - Begin

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors();
        app.UseOutputCache();
        //defining MapGroup
        var EndpointsGenre = app.MapGroup("/genre");

        #region CRUD
        //Create
        EndpointsGenre.MapPost("/", Insert);
        //Get All
        EndpointsGenre.MapGet("/", GetAll).CacheOutput(x => x.Expire(TimeSpan.FromSeconds(15)).Tag("cache-genre"));
        //Get an Entity
        EndpointsGenre.MapGet("/{id:int}", GetById);
        //Edit
        EndpointsGenre.MapPut("/{id:int}",Update);
        //Delete
        //***   "/genre (in MapGroup added)  /{id:int}"
        EndpointsGenre.MapDelete("/{id:int}", Delete);
        #endregion

        // Middleware zone - End
        app.Run();

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
        };
        //*****

        //Create
        static async Task<Created<Genre>> Insert (Genre genre, IOutputCacheStore iCache, IGenresRepository repository)
        {
            var id = await repository.Create(genre);
            await iCache.EvictByTagAsync("cache-genre", default);
            return TypedResults.Created($"/genre/{id}", genre);
        };
        //******

        //Update
        static async Task<Results<NotFound,NoContent>> Update(int ID, Genre genre, IGenresRepository repository
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
        };
        //Delete
        static async Task<Results<NotFound,NoContent>> Delete(IGenresRepository repository, int ID
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
        };
        #endregion
    }
}


