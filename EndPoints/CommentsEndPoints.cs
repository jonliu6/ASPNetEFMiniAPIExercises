using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;
using MinimalAPIsWithASPNetEF.Repositories;

namespace MinimalAPIsWithASPNetEF.EndPoints
{
    public static class CommentsEndPoints
    {
        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
        {
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("comments-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Results<Created<CommentDTO>, NotFound>> Create(int movieId, CreateCommentDTO createCommentDto,
            IMoviesRepository mRepo, ICommentsRepository cRepo, IMapper mapper, IOutputCacheStore outCacheStore)
        {
            // check if a movie exists for the new comments
            if (!await mRepo.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var comment = mapper.Map<Comment>(createCommentDto);
            comment.MovieId = movieId;
            var id = await cRepo.Create(comment);
            await outCacheStore.EvictByTagAsync("comments-get", default);
            var commentDto = mapper.Map<CommentDTO>(comment);
            return TypedResults.Created($"/comments/{id}", commentDto);
        }

        static async Task<Results<Ok<List<CommentDTO>>, NotFound>> GetAll(int movieId, ICommentsRepository cRepo, IMoviesRepository mRepo,
            IMapper mapper, int page = 1, int recordsPerPage = 10)
        {
            // check if a movie exists for the new comments
            if (!await mRepo.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var comments = await cRepo.GetAll(movieId);
            var commentDtos = mapper.Map<List<CommentDTO>>(comments);
            return TypedResults.Ok(commentDtos);
        }

        static async Task<Results<Ok<CommentDTO>, NotFound>> GetById(int movieId, int id, ICommentsRepository cRepo, IMoviesRepository mRepo,
            IMapper mapper)
        {
            // check if a movie exists for the new comments
            if (!await mRepo.Exists(movieId))
            {
                return TypedResults.NotFound();
            }
            var comment = await cRepo.GetById(id);
            if (comment is null)
            {
                return TypedResults.NotFound();
            }
            var commentDto = mapper.Map<CommentDTO>(comment);
            return TypedResults.Ok(commentDto);
        }

        static async Task<Results<NoContent, NotFound>> Update(int movieId, int id, CreateCommentDTO createCommentDto, ICommentsRepository cRepo, IMoviesRepository mRepo,
            IMapper mapper, IOutputCacheStore outCacheStore)
        {
            // check if a movie exists
            if (!await mRepo.Exists(movieId))
            {
                return TypedResults.NotFound();
            }
            // check if a comment exists 
            if (!await cRepo.Exists(id))
            {
                return TypedResults.NotFound();
            }

            var comment = mapper.Map<Comment>(createCommentDto);
            comment.Id = id;
            comment.MovieId = movieId;

            await cRepo.Update(comment);
            await outCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int movieId, int id, ICommentsRepository cRepo, IMoviesRepository mRepo,
            IMapper mapper, IOutputCacheStore outCacheStore)
        {
            // check if a movie exists
            if (!await mRepo.Exists(movieId))
            {
                return TypedResults.NotFound();
            }
            // check if a comment exists 
            if (!await cRepo.Exists(id))
            {
                return TypedResults.NotFound();
            }

            await cRepo.Delete(id);
            await outCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }
    }
}
