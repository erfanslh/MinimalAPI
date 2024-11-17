using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;
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
            routeGroup.MapPost("/", Create).DisableAntiforgery();
            return routeGroup;
        }
        //We use [FormForm] cause we have file in our CreateActorDTO entity
        // Task<Type-of-Task <The template we want to return>>
        static async Task<Created<ActorDTO>> Create([FromForm] CreateActorDTO createActorDTO,
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
    }
}
