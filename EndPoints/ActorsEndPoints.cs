using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.FileProviders;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;
using MinimalAPIsWithASPNetEF.Filters;
using MinimalAPIsWithASPNetEF.Repositories;
using MinimalAPIsWithASPNetEF.Services;
using System.ComponentModel;

namespace MinimalAPIsWithASPNetEF.EndPoints
{
    public static class ActorsEndPoints
    {
        private readonly static string container = "actors"; // have an actors folder
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromMinutes(5)).Tag("actors-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/getByName/{name}", GetByName); // URL: http://<server>:<port>/actors/getByName/{name}
            group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<GenericValidationFilter<CreateActorDTO>>(); // antiForgery is used in MVC but not miniAPIs, so disable it here
            group.MapPut("/{id:int}", Update).DisableAntiforgery(); // http://<server>:<port>/actors/{id}
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Ok<List<ActorDTO>>> GetAll(IActorsRepository repo, IMapper mapper, int page = 1, int recordsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, RecordsPerPage = recordsPerPage };
            var actors = await repo.GetAll(pagination);
            var actorsDto = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDto);
        }

        static async Task<Ok<List<ActorDTO>>> GetByName(string name, IActorsRepository repo, IMapper mapper)
        {
            var actors = await repo.GetByName(name);
            var actorsDto = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDto);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> GetById(int id, IActorsRepository repo, IMapper mapper)
        {
            var actor = await repo.GetById(id);
            if (actor is null)
            {
                return TypedResults.NotFound();
            }

            var actorsDto = mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(actorsDto);
        }

        // Task<Results<Created<ActorDTO>, ValidationProblem>>
        static async Task<Created<ActorDTO>> Create([FromForm] CreateActorDTO createActorDto,
            IActorsRepository repo, IOutputCacheStore outCacheStore, IMapper mapper,
            // IValidator<CreateActorDTO> validator,
            IFileStorage fileStorage
            )
        {
            // moved to GenericValidationFilter
            //var validationResult = await validator.ValidateAsync(createActorDto);
            //if (!validationResult.IsValid)
            //{
            //    return TypedResults.ValidationProblem(validationResult.ToDictionary());
            //}

            var actor = mapper.Map<Actor>(createActorDto);
            if (createActorDto.Picture is not null)
            {
                var url = await fileStorage.Store(container, createActorDto.Picture);
                actor.Picture = url;
            }
            var id = await repo.Create(actor);
            await outCacheStore.EvictByTagAsync("actors-get", default); // Tag for Evict
            var actorDto = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actors/{id}", actorDto);
        }

        /// <summary>
        /// Update actor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createActorDto"></param>
        /// <param name="repo"></param>
        /// <param name="fileStorage"></param> for user to update picture
        /// <param name="outCacheStore"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateActorDTO createActorDto,
            IActorsRepository repo, IFileStorage fileStorage, IOutputCacheStore outCacheStore, IMapper mapper
            )
        {
            // find the original record
            var actorDB = await repo.GetById(id);
            if (actorDB is null)
            {
                return TypedResults.NotFound(); // Http status code 404
            }
            var actorForUpdate = mapper.Map<Actor>(createActorDto);
            actorForUpdate.Id = id;
            actorForUpdate.Picture = actorDB.Picture;

            // user wants to update a picture
            if (createActorDto.Picture is not null)
            {
                var url = await fileStorage.Edit(actorForUpdate.Picture, container, createActorDto.Picture);
                actorForUpdate.Picture = url;
            }

            await repo.Update(actorForUpdate);
            await outCacheStore.EvictByTagAsync("actors-get", default);

            return TypedResults.NoContent(); // Http status code 204
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IActorsRepository repo, 
            IFileStorage fileStorage, IOutputCacheStore outCacheStore
            )
        {
            // find the original record
            var actorDB = await repo.GetById(id);
            if (actorDB is null)
            {
                return TypedResults.NotFound(); // Http status code 404
            }
            await repo.Delete(id);
            await fileStorage.Delete(actorDB.Picture, container); // delete the picture url
            await outCacheStore.EvictByTagAsync("actors-get", default);

            return TypedResults.NoContent(); // Http status code 204
        }
    }
}
