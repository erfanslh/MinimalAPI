using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.EndPoints
{
    public static class CommentEndpoints
    {
        public static RouteGroupBuilder MapComment(this RouteGroupBuilder routeGroup)
        {
            routeGroup.MapPost("/", Create);
            routeGroup.MapGet("/", GetAll)
                    .CacheOutput(c=> c.Expire(TimeSpan.FromSeconds(60)).Tag("comment-get"));
            routeGroup.MapGet("/{id:int}", GetById);
            return routeGroup;
        }

        static async Task<IResult> GetAll(int movieID, ICommentRepository commentRepository,
            IMovieRepository movieRepository, IMapper mapper)
        {
            if (!await movieRepository.Exists(movieID))
            {
                return TypedResults.NotFound();
            }
            var comment = await commentRepository.GetAll(movieID);
            var commentDTO = mapper.Map<List<CommentDTO>>(comment);
            return TypedResults.Ok(commentDTO);
        }

        static async Task<Results<Ok<CommentDTO>,NotFound>> GetById(int movieID, int id, ICommentRepository commentRepository,
            IMovieRepository movieRepository, IMapper mapper)
        {
            if (!await movieRepository.Exists(movieID))
            {
                return TypedResults.NotFound();
            }
            var comment = await commentRepository.GetByID(id);
            if (comment ==null)
            {
                return TypedResults.NotFound();
            }
            var commentDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Ok(commentDTO);
        }
        static async Task< Results<Created<CommentDTO>,NotFound> > Create(int movieID, CreateCommentDTO createCommentDTO,
                    IMapper mapper, IOutputCacheStore cache, 
                    ICommentRepository commentRepository, IMovieRepository movieRepository )
        {
            // Detect wether comment in the Movie exists
            if (! await movieRepository.Exists(movieID))
            {
                return TypedResults.NotFound();
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.MovieId = movieID;
            var id = await commentRepository.Create(comment);
            await cache.EvictByTagAsync("comment-get", default);
            var finalMapComment = mapper.Map<CommentDTO>(comment);
            return TypedResults.Created($"/comment/{id}", finalMapComment);

        }
    }
}
