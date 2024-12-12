using FluentAssertions.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPIMoviez;
using MinimalAPIMoviez.EndPoints;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;
using System.Collections.Generic;
using FluentValidation;

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
        //Scope-Service for Repositories and Interfaces
        builder.Services.AddScoped<IGenresRepository, GenresRepository>();
        builder.Services.AddScoped<IActorRepository, ActorRepository>();
        builder.Services.AddScoped<IMovieRepository, MovieRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();

        builder.Services.AddTransient<IFileStorage, AzureStorage>();
        builder.Services.AddAutoMapper(typeof(Program));
        //Add this service to use HttpContextAccessor
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        //Service Zone - End

        var app = builder.Build();

        // Middleware zone - Begin

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors();
        app.UseOutputCache();
        //defining MapGroup
        app.MapGroup("/genre").MapGenres();
        app.MapGroup("/actor").MapActors();
        app.MapGroup("/movie").MapMovies();

        // We get Comments on a Movie so first we need the ID of the Movie then get into comments
        app.MapGroup("/movie/{movieId:int}/comments").MapComment();


        // Middleware zone - End
        app.Run();


    }
}


