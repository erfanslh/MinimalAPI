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
                .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("actors-get")).AddPaginationParameters().WithOpenApi(options=>
                {
                    options.Summary = "Get all Actors";
                    options.Description = "Click on Execute to receive all stored actors in the Database";
                    options.Parameters[0].Description = "Page Number";
                    options.Parameters[1].Description = "Count of Actors per page";
                    return options;
                });

            routeGroup.MapGet("/{id:int}", GetById).WithOpenApi(options =>
            {
                options.Summary = "Get an Actor by it's ID";
                options.Description = "by giving actor's ID, you'll receive all information about the actor";
                options.Parameters[0].Description = "ID of the Actor";
                return options;
            });
            routeGroup.MapGet("getByName/{name}", GetByName).WithOpenApi(options =>
            {
                options.Summary = "Get an Actor by it's Name";
                options.Description = "You can retrieves all information from an actor, by giving it's name";
                options.Parameters[0].Description = "Name of the Actor";
                return options;
            }); 
            routeGroup.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateActorDTO>>()
                .RequireAuthorization("isadmin").WithOpenApi(options =>
                {
                    options.Summary = "Create an Actor";
                    options.Description = "to create an Actor, first you need to be authorization as an ADMIN, then you can add the Actor";
                    options.RequestBody.Description = "Add Information of the Actor you want to create";
                    return options;
            }); 
            routeGroup.MapPut("/{id:int}", Update).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateActorDTO>>()
                .RequireAuthorization("isadmin").WithOpenApi(options =>
                {
                    options.Summary = "Update an Actor";
                    options.Description = "to update the Actor, first you need to be authorization as an ADMIN, then you can update the Actor using it's ID";
                    options.Parameters[0].Description = "ID of the Actor you want to EDIT";
                    options.RequestBody.Description = "Edit the Information of the Actor you want to Update";
                    return options;
                });
            routeGroup.MapDelete("/{id:int}", Delete).RequireAuthorization("isadmin").WithOpenApi(options =>
            {
                options.Summary = "Delete an Actor";
                options.Description = "to delete an Actor, first you need to be authorization as an ADMIN, then you can delete the Actor";
                options.Parameters[0].Description = "ID of the Actor you want to DELETE";
                return options;
            }); ; ;
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
