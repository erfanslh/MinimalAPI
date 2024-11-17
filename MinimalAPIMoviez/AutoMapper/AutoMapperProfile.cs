using AutoMapper;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // These Mapping-Addresses are very Important, we should use them correclty
            // Otherwise it doesnt work anymore with Database
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();

            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateActorDTO, Actor>()
            //Imagename in CreateActorDTO is "IFormFile", which should be ignored during Mapping
            .ForMember(m => m.Imagename, option => option.Ignore());
        }
    }
}
