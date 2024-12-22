using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Filters;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;
using System.Runtime.CompilerServices;

namespace MinimalAPIMoviez.EndPoints
{

    public static class ActorsEndpoints
    {

        private readonly static string container = "actor";
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder routeGroup)
        {
            routeGroup.MapGet("/", GetAll)
                .CacheOutput(c=> c.Expire(TimeSpan.FromMinutes(1)).Tag("actors-get"));
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

        static async Task<Ok<List<ActorDTO>>> GetAll(IActorRepository repository, IMapper mapper,
                int page =1, int recordsperpage = 10)
        {
            var pagination = new PaginationDTO { Page = page,RecordsPerPage = recordsperpage };
            var allActors = await repository.GetAll(pagination);
            var map = mapper.Map<List<ActorDTO>>(allActors);
            return TypedResults.Ok(map);
        }
        #endregion

        #region Get Actors by ID
        static async Task<Results<Ok<ActorDTO>, NotFound>> GetById(int id, IActorRepository repository, IMapper mapper)
        {
            var findActor = await repository.GetByID(id);
            if (findActor is not null)
            {
                var map = mapper.Map<ActorDTO>(findActor);
                return TypedResults.Ok(map);
            }
            return TypedResults.NotFound();
        }
        #endregion

        #region Filter Actors by Name
        static async Task<Ok<List<ActorDTO>>> GetByName(string name, IActorRepository repository, IMapper mapper)
        {
            var actorName = await repository.GetByName(name);
            var dto = mapper.Map<List<ActorDTO>>(actorName);
            return TypedResults.Ok(dto);
        }
        #endregion

        #region Create Actor
        //We use [FormForm] cause we have file in our CreateActorDTO entity
        // Task<Type-of-Task <The template we want to return>>
        static async Task<IResult> Create([FromForm] CreateActorDTO createActorDTO,
            IMapper mapper,
            IActorRepository repository,
            IOutputCacheStore outputCache,
            IFileStorage fileStorage)
        {
            var actor = mapper.Map<Actor>(createActorDTO);
            if (createActorDTO.Imagename is not null)
            {
                var url = await fileStorage.Store(container, createActorDTO.Imagename);
                actor.Imagename = url;
            }
            var id = await repository.Create(actor);
            await outputCache.EvictByTagAsync("actors-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actor/{id}", actorDTO);

        }
        #endregion

        #region Update an Actor
        static async Task<Results<NotFound,NoContent>> Update(int id, IActorRepository repository, [FromForm]CreateActorDTO createActorDTO,
            IMapper mapper, IOutputCacheStore cacheStore, IFileStorage fileStorage)
        {
            var findActor = await repository.GetByID(id);
            if (findActor is null)
            {
                return TypedResults.NotFound();
            }

            var editActor = mapper.Map<Actor>(createActorDTO);
            editActor.Id = id;
            editActor.Imagename = findActor.Imagename;

            if (createActorDTO.Imagename is not null)
            {
                var url = await fileStorage.Update(editActor.Imagename, container, createActorDTO.Imagename);
                editActor.Imagename = url;
            }

            await repository.Update(editActor);
            await cacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();

        }
        #endregion

        #region Delete an Actor

        static async Task<Results<NotFound, NoContent>> Delete(int id, IActorRepository repository,
                IOutputCacheStore cacheStore, IFileStorage fileStorage
           )
        {
            var findActor = await repository.GetByID(id);
            if (findActor is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);

            await fileStorage.Delete(findActor.Imagename, container);

            await cacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }

        #endregion

    }
}
