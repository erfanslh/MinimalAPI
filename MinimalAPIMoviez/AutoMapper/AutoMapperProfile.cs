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
        }
    }
}
