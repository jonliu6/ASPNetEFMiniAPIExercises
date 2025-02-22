using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;
using MinimalAPIsWithASPNetEF.Filters;
using MinimalAPIsWithASPNetEF.Repositories;
using MinimalAPIsWithASPNetEF.Services;

namespace MinimalAPIsWithASPNetEF.EndPoints
{
    public static class GenresEndPoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<GenreValidationFilter>();
            group.MapGet("/", GetAll).CacheOutput(g => g.Expire(TimeSpan.FromMinutes(5)).Tag("genres-get"));
            // use a custom filter 
            group.MapGet("/{id:int}", GetById); //.AddEndpointFilter<TestFilter>();
            group.MapPut("/{id:int}", Update).DisableAntiforgery().AddEndpointFilter<GenericValidationFilter<CreateGenreDTO>>(); // http://<server>:<port>/genres/{id}
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Results<Created<GenreDTO>, NotFound>> Create(CreateGenreDTO createGenreDto, 
            IGenresRepository repo, IMapper mapper, 
            // IValidator<CreateGenreDTO> validator,
            IOutputCacheStore outCacheStore)
        {
            // validation for createGenreDto. The following validation logic is moved to GenreValidationFilter
            //var validationResult = await validator.ValidateAsync(createGenreDto);
            //if (!validationResult.IsValid)
            //{
            //    return TypedResults.ValidationProblem(validationResult.ToDictionary());
            //}

            var genre = mapper.Map<Genre>(createGenreDto);
            var id = await repo.Create(genre);
            await outCacheStore.EvictByTagAsync("genres-get", default);
            var genreDto = mapper.Map<GenreDTO>(genre);
            return TypedResults.Created($"/genres/{id}", genreDto);
        }

        static async Task<Ok<List<GenreDTO>>> GetAll(IGenresRepository repo, IMapper mapper, int page = 1, int recordsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, RecordsPerPage = recordsPerPage };
            var genres = await repo.GetAll(pagination);
            var genreDto = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genreDto);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenresRepository repo, IMapper mapper)
        {
            var genre = await repo.GetById(id);
            if (genre is null)
            {
                return TypedResults.NotFound();
            }

            var genreDto = mapper.Map<GenreDTO>(genre);
            return TypedResults.Ok(genreDto);
        }

        // Task<Results<NoContent, NotFound, ValidationProblem>>
        static async Task<Results<NoContent, NotFound>> Update(int id, CreateGenreDTO createGenreDto,
            IGenresRepository repo, IOutputCacheStore outCacheStore,
            // IValidator<CreateGenreDTO> validator,
            IMapper mapper
            )
        {
            // validation for createGenreDto. The following logic is moved to GenericValidationFilter
            //var validationResult = await validator.ValidateAsync(createGenreDto);
            //if (!validationResult.IsValid)
            //{
            //    return TypedResults.ValidationProblem(validationResult.ToDictionary());
            //}

            // find the original record
            var genre = await repo.GetById(id);
            if (genre is null)
            {
                return TypedResults.NotFound(); // Http status code 404
            }
            var genrerForUpdate = mapper.Map<Genre>(createGenreDto);
            genrerForUpdate.Id = id;
            
            await repo.Update(genrerForUpdate);
            await outCacheStore.EvictByTagAsync("genres-get", default);

            return TypedResults.NoContent(); // Http status code 204
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IGenresRepository repo,
            IOutputCacheStore outCacheStore)
        {
            // find the original record
            var genre = await repo.GetById(id);
            if (genre is null)
            {
                return TypedResults.NotFound(); // Http status code 404
            }
            await repo.Delete(id);
            await outCacheStore.EvictByTagAsync("genres-get", default);

            return TypedResults.NoContent(); // Http status code 204
        }
    }
}
