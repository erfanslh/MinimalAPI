using AutoMapper;
using Hl7.Fhir.Utility;
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
            return routeGroup;
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
