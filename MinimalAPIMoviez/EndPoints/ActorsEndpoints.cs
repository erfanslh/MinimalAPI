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

        private readonly static string defaultActorName = "Nophoto";
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder routeGroup)
        { 
            routeGroup.MapPost("/", Create).DisableAntiforgery();
            return routeGroup;
        }
        //We use [FormForm] cause we have file in our CreateActorDTO entity
        // Task<Type-of-Task <The template we want to return>>
        static async Task<Created<ActorDTO>> Create([FromForm] CreateActorDTO actorDTO,
            IMapper mapper,
            IActorRepository repository,
            IOutputCacheStore outputCache,
            IFileStorage fileStorage)
        {
            var mapActor = mapper.Map<Actor>(actorDTO);
            var id = await repository.Create(mapActor);
            if (fileStorage != null)
            {
                var url = await fileStorage.Store(defaultActorName, actorDTO.Imagename);
                mapActor.Imagename = url;
            }
            await outputCache.EvictByTagAsync("actors-get", default);

            var actor = mapper.Map<ActorDTO>(id);
            return TypedResults.Created($"/actor/{id}", actor);

            
            
        }
    }
}
