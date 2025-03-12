using AutoMapper;
using HotChocolate.Authorization;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;
using MinimalAPIsWithASPNetEF.Repositories;

namespace MinimalAPIsWithASPNetEF.GraphQL
{
    public class Mutation
    {
        [Serial]
        [Authorize(Policy = "isAdmin")] // with GraphQL, go to connection settings > Authorization > Type=Bearer, Token=...
        public async Task<GenreDTO> InsertGenre([Service] IGenresRepository repo, [Service] IMapper mapper, CreateGenreDTO createDto)
        {
            var genre = mapper.Map<Genre>(createDto);
            await repo.Create(genre);
            var genreDto = mapper.Map<GenreDTO>(genre);
            return genreDto;
        }
    }
}
