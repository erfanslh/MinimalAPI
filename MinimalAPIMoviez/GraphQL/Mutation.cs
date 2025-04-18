using AutoMapper;
using HotChocolate.Authorization;
using MinimalAPIMoviez.DTOs;
using MinimalAPIMoviez.Entities;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.GraphQL
{
    public class Mutation
    {
        [Serial]
        [Authorize(Policy ="isadmin")]
        public async Task<GenreDTO> InsertGenre([Service] IGenresRepository repository, [Service] IMapper mapper, CreateGenreDTO createGenreDTO)
        {
            var genre = mapper.Map<Genre>(createGenreDTO);
            await repository.Create(genre);
            var genreDTO = mapper.Map<GenreDTO>(genre);
            return genreDTO;
        }
    }
}

