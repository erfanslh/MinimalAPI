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
        app.MapGroup("/genre").MapGenres();



        // Middleware zone - End
        app.Run();


    }
}


