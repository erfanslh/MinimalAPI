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
using Microsoft.OpenApi.Models;
using MinimalAPIMoviez.Swagger;
using Error = MinimalAPIMoviez.Entities.Error;
using MinimalAPIMoviez.GraphQL;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
        var redisConnection = builder.Configuration.GetConnectionString("Redis");
        var allowedOriginsValue = builder.Configuration["AllowedOrigin"];
        var allowedOrigins = string.IsNullOrWhiteSpace(allowedOriginsValue)
            ? Array.Empty<string>()
            : allowedOriginsValue
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // Service Zone - Start
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(defaultConnection, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }));

        builder.Services.AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddAuthorization()
            .AddProjections()
            .AddFiltering()
            .AddSorting();

        builder.Services.AddIdentityCore<IdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddScoped<UserManager<IdentityUser>>();
        builder.Services.AddScoped<SignInManager<IdentityUser>>();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (allowedOrigins.Length == 0 ||
                    (allowedOrigins.Length == 1 && allowedOrigins[0] == "*"))
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
                else
                {
                    policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
                }
            });
        });

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            builder.Services.AddStackExchangeRedisOutputCache(options =>
            {
                options.Configuration = redisConnection;
            });
        }
        else
        {
            builder.Services.AddOutputCache();
        }

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Movie API",
                Description = "The Movie API Application is a lightweight and efficient Minimal API built with ASP.NET Core, designed to manage and provide movie-related data.",
                Contact = new OpenApiContact
                {
                    Email = "Erfan.Slh@yahoo.com",
                    Name = "Erfan Mollasalehi",
                    Url = new Uri("https://www.linkedin.com/in/erfan-mollasalehi/")
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
            });
            options.OperationFilter<AuthorizationFilter>();
        });

        builder.Services.AddScoped<IGenresRepository, GenresRepository>();
        builder.Services.AddScoped<IActorRepository, ActorRepository>();
        builder.Services.AddScoped<IMovieRepository, MovieRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddScoped<IErrorRepository, ErrorRepository>();

        var storageProvider = builder.Configuration["Storage:Provider"] ?? "Local";
        if (string.Equals(storageProvider, "Azure", StringComparison.OrdinalIgnoreCase))
        {
            builder.Services.AddTransient<IFileStorage, AzureStorage>();
        }
        else
        {
            builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
        }

        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IUserServices, UserServices>();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddProblemDetails();

        builder.Services.AddAuthentication().AddJwtBearer(option =>
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

        builder.Services.AddAuthorization(option =>
        {
            option.AddPolicy("isadmin", policy => policy.RequireClaim("isadmin"));
        });

        // Service Zone - End

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseStaticFiles();

        app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandlerFeature?.Error!;
            var error = new Error
            {
                Date = DateTime.UtcNow,
                ErrorMessage = exception.Message,
                StackTrace = exception.StackTrace
            };

            var repository = context.RequestServices.GetRequiredService<IErrorRepository>();
            await repository.Create(error);

            await Results.BadRequest(new
            {
                type = "Error",
                message = "an unexpected exception has occured",
                status = 500
            }).ExecuteAsync(context);
        }));

        app.UseStatusCodePages();
        app.UseCors();
        app.UseOutputCache();
        app.UseAuthentication();
        app.UseAuthorization();

        await ApplyMigrationsWithRetryAsync(app);

        app.MapGraphQL();

        app.MapGet("/error", () =>
        {
            throw new InvalidOperationException("Error occurs on /error MapGet");
        });

        app.MapGroup("/genre").MapGenres();
        app.MapGroup("/actor").MapActors();
        app.MapGroup("/movie").MapMovies();
        app.MapGroup("/users").MapUser();
        app.MapGroup("/movie/{movieId:int}/comments").MapComment();

        await app.RunAsync();
    }

    private static async Task ApplyMigrationsWithRetryAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigration");
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        const int maxRetries = 15;
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                logger.LogWarning(ex, "Database not ready yet. Retry {Attempt}/{MaxRetries} in 5 seconds.", attempt, maxRetries);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        await dbContext.Database.MigrateAsync();
    }
}
