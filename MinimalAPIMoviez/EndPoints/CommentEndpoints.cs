using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.DTOs.CommentRequestDTO;
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

        #region GetAll
        static async Task<IResult> GetAll([AsParameters] GetAllCommentsRequestDTO model)
        {
            if (!await model.MovieRepository.Exists(model.MovieID))
            {
                return TypedResults.NotFound();
            }
            var comment = await model.CommentRepository.GetAll(model.MovieID);
            var commentDTO = model.Mapper.Map<List<CommentDTO>>(comment);
            return TypedResults.Ok(commentDTO);
        }
        #endregion

        #region GetByID
        static async Task<Results<Ok<CommentDTO>,NotFound>> GetById([AsParameters] GetByIDCommentsRequestDTO model)
        {
            if (!await model.MovieRepository.Exists(model.MovieID))
            {
                return TypedResults.NotFound();
            }
            var comment = await model.CommentRepository.GetByID(model.ID);
            if (comment ==null)
            {
                return TypedResults.NotFound();
            }
            var commentDTO = model.Mapper.Map<CommentDTO>(comment);
            return TypedResults.Ok(commentDTO);
        }
        #endregion

        #region Update
        static async Task<IResult> Update(CreateCommentDTO createComment, [AsParameters] UpdateCommentsRequestDTO model)
        {
            if (!await model.MovieRepository.Exists(model.MovieID))
            {
                return TypedResults.NotFound();
            }
            var commentFromDB = await model.CommentRepository.GetByID(model.ID);
            if (commentFromDB is null)
            {
                return TypedResults.NotFound();
            }
            var user = await model.UserServices.GetUser();
            if (user is null)
            {
                return TypedResults.NotFound();
            }
            if (commentFromDB.UserId != user.Id)
            {
                return TypedResults.Forbid();
            }
            commentFromDB.Body = createComment.Body;

            await model.CommentRepository.Update(commentFromDB);
            await model.CacheStore.EvictByTagAsync("comment-get", default);
            return TypedResults.NoContent();
        }
        #endregion

        #region Delete
        static async Task<IResult> Delete([AsParameters] DeleteCommentsRequestDTO model)
        {
            if (!await model.CommentRepository.Exists(model.ID))
            {
                return TypedResults.NotFound();
            }
            // get comment by id
            var commentFromDB = await model.CommentRepository.GetByID(model.ID);
            if (commentFromDB is null)
            {
                return TypedResults.NotFound();
            }
            //logged in User
            var user = await model.UserServices.GetUser();
            if (user is null)
            {
                return TypedResults.NotFound();
            }
            if (commentFromDB.UserId != user.Id)
            {
                // if the user is not owner of comment
                return TypedResults.Forbid();
            }


            await model.CommentRepository.Delete(model.ID);
            await model.CacheStore.EvictByTagAsync("comment-get", default);
            return TypedResults.NoContent();

        }

        #endregion

        #region Create
        static async Task< Results<Created<CommentDTO>,NotFound,BadRequest<string>>> Create
            (CreateCommentDTO createCommentDTO, [AsParameters] CreateCommentsRequestDTO model)
        {
            // Detect wether comment in the Movie exists
            if (! await model.MovieRepository.Exists(model.MovieID))
            {
                return TypedResults.NotFound();
            }
            var user = await model.UserServices.GetUser();
            if (user is null)
            {
               return TypedResults.BadRequest("user not found");
            }

            var comment = model.Mapper.Map<Comment>(createCommentDTO);
            comment.MovieId = model.MovieID;
            comment.UserId = user.Id;
            var id = await model.CommentRepository.Create(comment);
            await model.CacheStore.EvictByTagAsync("comment-get", default);
            var finalMapComment = model.Mapper.Map<CommentDTO>(comment);
            return TypedResults.Created($"/comment/{id}", finalMapComment);

        }
        #endregion
    }
}
