using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Any;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.DTOs.ActorRequestDTO;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Filters;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;
using System.Runtime.CompilerServices;
using MinimalAPIMoviez.Utilities;
namespace MinimalAPIMoviez.EndPoints
{

    public static class ActorsEndpoints
    {

        private readonly static string container = "actor";
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder routeGroup)
        {
            routeGroup.MapGet("/", GetAll)
                .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("actors-get")).AddPaginationParameters();

            routeGroup.MapGet("/{id:int}", GetById);
            routeGroup.MapGet("getByName/{name}", GetByName);
            routeGroup.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateActorDTO>>()
                .RequireAuthorization("isadmin");
            routeGroup.MapPut("/{id:int}", Update).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateActorDTO>>()
                .RequireAuthorization("isadmin");
            routeGroup.MapDelete("/{id:int}", Delete).RequireAuthorization("isadmin");
            return routeGroup;
        }

        #region Get all Actors

        static async Task<Ok<List<ActorDTO>>> GetAll([AsParameters] GetAllActorRequestDTO model, PaginationDTO pagination)
        {
            //var pagination = new PaginationDTO { Page = model.Page,RecordsPerPage = model.RecordsPerPage };
            var allActors = await model.Repository.GetAll(pagination);
            var map = model.Mapper.Map<List<ActorDTO>>(allActors);
            return TypedResults.Ok(map);
        }
        #endregion

        #region Get Actors by ID
        static async Task<Results<Ok<ActorDTO>, NotFound>> GetById([AsParameters] GetByIdActorRequestDTO model)
        {
            var findActor = await model.Repository.GetByID(model.ID);

            if (findActor is not null)
            {
                var map = model.Mapper.Map<ActorDTO>(findActor);
                return TypedResults.Ok(map);
            }
            return TypedResults.NotFound();
        }
        #endregion

        #region Filter Actors by Name
        static async Task<Ok<List<ActorDTO>>> GetByName([AsParameters] GetByNameActorRequestDTO model)
        {
            var actorName = await model.Repository.GetByName(model.Name);
            var dto = model.Mapper.Map<List<ActorDTO>>(actorName);
            return TypedResults.Ok(dto);
        }
        #endregion

        #region Create Actor
        //We use [FormForm] cause we have file in our CreateActorDTO entity
        // Task<Type-of-Task <The template we want to return>>
        static async Task<IResult> Create([FromForm] CreateActorDTO createActorDTO, [AsParameters] CreateActorRequestDTO model )
        {
            var actor = model.Mapper.Map<Actor>(createActorDTO);
            if (createActorDTO.Imagename is not null)
            {
                var url = await model.FileStorage.Store(container, createActorDTO.Imagename);
                actor.Imagename = url;
            }
            var id = await model.Repository.Create(actor);
            await model.CacheStore.EvictByTagAsync("actors-get", default);
            var actorDTO = model.Mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actor/{id}", actorDTO);

        }
        #endregion

        #region Update an Actor
        static async Task<Results<NotFound,NoContent>> Update([FromForm]CreateActorDTO createActorDTO, [AsParameters] UpdateActorRequestDTO model)
        {
            var findActor = await model.Repository.GetByID(model.ID);
            if (findActor is null)
            {
                return TypedResults.NotFound();
            }

            var editActor = model.Mapper.Map<Actor>(createActorDTO);
            editActor.Id = model.ID;
            editActor.Imagename = findActor.Imagename;

            if (createActorDTO.Imagename is not null)
            {
                var url = await model.FileStorage.Update(editActor.Imagename, container, createActorDTO.Imagename);
                editActor.Imagename = url;
            }

            await model.Repository.Update(editActor);
            await model.CacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();

        }
        #endregion

        #region Delete an Actor

        static async Task<Results<NotFound, NoContent>> Delete([AsParameters] DeleteActorRequestDTO model )
        {
            var findActor = await model.Repository.GetByID(model.ID);
            if (findActor is null)
            {
                return TypedResults.NotFound();
            }

            await model.Repository.Delete(model.ID);

            await model.FileStorage.Delete(findActor.Imagename, container);

            await model.CacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }

        #endregion

    }
}
