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
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIMoviez.Utilities;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Service Zone - Start

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        #region Services for Authentication Users
        builder.Services.AddIdentityCore<IdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddScoped<UserManager<IdentityUser>>();
        builder.Services.AddScoped<SignInManager<IdentityUser>>();
        #endregion

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
        builder.Services.AddScoped<IErrorRepository, ErrorRepository>();

        builder.Services.AddTransient<IFileStorage, AzureStorage>();
        builder.Services.AddAutoMapper(typeof(Program));
        //Add this service to use HttpContextAccessor
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IUserServices, UserServices>();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services.AddProblemDetails();

        builder.Services.AddAuthentication().AddJwtBearer
                (option =>
                {
                    option.MapInboundClaims = false;

                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        IssuerSigningKeys = KeysHandler.GetAllKeys(builder.Configuration)
                    };
                });
        builder.Services.AddAuthorization (option => option.AddPolicy("isadmin", policy => 
                                                                            policy.RequireClaim("isadmin")));

        //Service Zone - End

        var app = builder.Build();

        // Middleware zone - Begin

        app.UseSwagger();
        app.UseSwaggerUI();
        // for Handling error
        app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandlerFeature?.Error!;
            var error = new Error();
            error.Date = DateTime.UtcNow;
            error.ErrorMessage = exception.Message;
            error.StackTrace = exception.StackTrace;

            var repository = context.RequestServices.GetRequiredService<IErrorRepository>();
            await repository.Create(error);

            await Results.BadRequest(new
            {
                type = "Error",
                message = "an unexpected exception has occured",
                statud = 500
            }).ExecuteAsync(context);
        })); 
        app.UseStatusCodePages(); //return a Status code, when an unhandle Exeption occurs.

        app.UseCors();
        app.UseOutputCache();
        app.UseAuthorization();
        app.MapGet("/error", () =>
        {
            throw new InvalidOperationException("Error occurs on /error MapGet");
        });
        //defining MapGroup
        app.MapGroup("/genre").MapGenres();
        app.MapGroup("/actor").MapActors();
        app.MapGroup("/movie").MapMovies();
        app.MapGroup("/users").MapUser();

        // We get Comments on a Movie so first we need the ID of the Movie then get into comments
        app.MapGroup("/movie/{movieId:int}/comments").MapComment();


        // Middleware zone - End
        app.Run();


    }
}


