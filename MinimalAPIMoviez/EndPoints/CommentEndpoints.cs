using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Filters;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;

namespace MinimalAPIMoviez.EndPoints
{
    public static class CommentEndpoints
    {
        public static RouteGroupBuilder MapComment(this RouteGroupBuilder routeGroup)
        {
            routeGroup.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>()
                .RequireAuthorization();
            routeGroup.MapGet("/", GetAll)
                    .CacheOutput(c=> c.Expire(TimeSpan.FromSeconds(60)).Tag("comment-get"));
            routeGroup.MapGet("/{id:int}", GetById);
            routeGroup.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>();
            routeGroup.MapDelete("/{id:int}", Delete);
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

        static async Task<IResult> Update(int movieID, int id, CreateCommentDTO createComment,
            ICommentRepository commentRepository, IOutputCacheStore cache,
            IMovieRepository movieRepository, IMapper mapper, IUserServices userServices)
        {
            if (!await movieRepository.Exists(movieID))
            {
                return TypedResults.NotFound();
            }
            var commentFromDB = await commentRepository.GetByID(id);
            if (commentFromDB is null)
            {
                return TypedResults.NotFound();
            }
            var user = await userServices.GetUser();
            if (user is null)
            {
                return TypedResults.NotFound();
            }
            if (commentFromDB.UserId != user.Id)
            {
                return TypedResults.Forbid();
            }
            commentFromDB.Body = createComment.Body;

            await commentRepository.Update(commentFromDB);
            await cache.EvictByTagAsync("comment-get", default);
            return TypedResults.NoContent();
        }

        static async Task<IResult> Delete (int movieID, int id, ICommentRepository commentRepository,
            IMovieRepository movieRepository, IOutputCacheStore cacheStore, IUserServices userServices)
        {
            if (!await commentRepository.Exists(id))
            {
                return TypedResults.NotFound();
            }
            // get comment by id
            var commentFromDB = await commentRepository.GetByID(id);
            if (commentFromDB is null)
            {
                return TypedResults.NotFound();
            }
            //logged in User
            var user = await userServices.GetUser();
            if (user is null)
            {
                return TypedResults.NotFound();
            }
            if (commentFromDB.UserId != user.Id)
            {
                // if the user is not owner of comment
                return TypedResults.Forbid();
            }



            await commentRepository.Delete(id);
            await cacheStore.EvictByTagAsync("comment-get", default);
            return TypedResults.NoContent();

        }
        static async Task< Results<Created<CommentDTO>,NotFound,BadRequest<string>>> Create(int movieID, CreateCommentDTO createCommentDTO,
                    IMapper mapper, IOutputCacheStore cache,IUserServices userServices ,
                    ICommentRepository commentRepository, IMovieRepository movieRepository )
        {
            // Detect wether comment in the Movie exists
            if (! await movieRepository.Exists(movieID))
            {
                return TypedResults.NotFound();
            }
            var user = await userServices.GetUser();
            if (user is null)
            {
               return TypedResults.BadRequest("user not found");
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.MovieId = movieID;
            comment.UserId = user.Id;
            var id = await commentRepository.Create(comment);
            await cache.EvictByTagAsync("comment-get", default);
            var finalMapComment = mapper.Map<CommentDTO>(comment);
            return TypedResults.Created($"/comment/{id}", finalMapComment);

        }
    }
}
