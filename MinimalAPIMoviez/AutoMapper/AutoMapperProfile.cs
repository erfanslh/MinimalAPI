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

            CreateMap<Movie, MovieDTO>()
                .ForMember(m => m.actorMovies, entity => 
                            entity.MapFrom(p => p.ActorsMovies.Select(am => new ActorMovieDTO { Character = am.Character, Id = am.ActorId, Name = am.Actor.Name })))
                .ForMember(m => m.genres, entity => entity
                           .MapFrom(p => p.GenresMovies.Select(gm => new GenreDTO { Id = gm.GenreId, Name = gm.Genres.Name })));
            CreateMap<CreateMovieDTO, Movie>()
            .ForMember(m => m.CoverImage, option => option.Ignore());

            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();

            CreateMap<ActorMovie, AssignActorMovieDTO>();
            CreateMap<AssignActorMovieDTO, ActorMovie>();
        }
    }
}
