using FluentAssertions.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPIMoviez;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;

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

        //Create
        app.MapPost("/genre", async (Genre genre, IOutputCacheStore iCache, IGenresRepository repository) =>
        {
            var id = await repository.Create(genre);
            await iCache.EvictByTagAsync("cache-genre", default);
            return Results.Created($"/genre/{id}", genre);
        });

        //Get All
        app.MapGet("/genre", async (IGenresRepository repository) =>
        {
            return await repository.GetAll();
        }).CacheOutput(x => x.Expire(TimeSpan.FromSeconds(15)).Tag("cache-genre"));


        //Get an Entity
        app.MapGet("/genre/{id:int}", async (IGenresRepository repository, int ID) =>
        {
            var genreId = await repository.GetbyID(ID);
            if (genreId == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(genreId);
        });

        #region Update
        //we use PUT for Update
        app.MapPut("/genre/{id:int}", async (int ID,Genre genre, IGenresRepository repository
            ,IOutputCacheStore cacheStore) =>
        {
            //*** here we should use "await" ==> cuz it has Async and we are working with DB
            var Exists = await repository.Exist(ID);
            if (!Exists)
            {
                return Results.NotFound();
            }
            await repository.Update(genre);
            // we Use IOutputCacheStore to cleanup the Caches by creating an Object of it
            // ("IOutputCacheStore cacheStore"), then we use the object to implement cleanup Cache
            await cacheStore.EvictByTagAsync("cache-genre", default);
            //cuz we dont return anything in Update so we use
            //    "Results.NoContent()"
            return Results.NoContent();
        });
        #endregion

        #region Delete
        //We do the same following commands as we did for Update
        //***   "/genre/{id:int}"
        app.MapDelete("/genre/{id:int}", async (IGenresRepository repository, int ID
            , IOutputCacheStore cacheStore) =>
        {
            var existing = await repository.Exist(ID);
            if (!existing)
            {
                return Results.NotFound();
            }

            await repository.Delete(ID);
            await cacheStore.EvictByTagAsync("cache-genre", default);
            return Results.NoContent();
        });
        #endregion
        // Middleware zone - End
        app.Run();
    }
}


