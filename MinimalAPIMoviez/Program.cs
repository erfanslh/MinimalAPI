using FluentAssertions.Common;
using Microsoft.AspNetCore.Builder;
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

        //**
        // ãØÇáÚå ÈÔå
        app.MapPost("/genre", async (Genre genre, IGenresRepository repository) =>
        {
            var id = await repository.Create(genre);
            return Results.Created($"/genre/{id}", genre);
        });

        //**




        app.MapGet("/genre", async (IGenresRepository repository) =>
        {
            return await repository.GetAll();
        }).CacheOutput(x=> x.Expire(TimeSpan.FromSeconds(15)));

        app.MapGet("/genre/{id:int}", async (int ID, IGenresRepository repository) =>
        {
            return await repository.GetByID(ID);
        });

        // Middleware zone - End
        app.Run();
    }
}