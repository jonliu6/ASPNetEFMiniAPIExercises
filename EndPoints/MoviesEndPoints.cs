using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;
using MinimalAPIsWithASPNetEF.Repositories;
using MinimalAPIsWithASPNetEF.Services;

namespace MinimalAPIsWithASPNetEF.EndPoints
{
    public static class MoviesEndPoints
    {
        private readonly static string container = "movies";

        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(g => g.Expire(TimeSpan.FromMinutes(5)).Tag("movies-get"));
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/getByTitle{title}", GetByTitle);
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Ok<List<MovieDTO>>> GetAll(IMoviesRepository repo, IMapper mapper, int page = 1, int recordsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, RecordsPerPage = recordsPerPage };
            var movies = await repo.GetAll(pagination);
            var movieDto = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(movieDto);
        }

        static async Task<Created<MovieDTO>> Create([FromForm] CreateMovieDTO createMovieDto,
            IMoviesRepository repo, IOutputCacheStore outCacheStore, IMapper mapper, IFileStorage fileStorage
            )
        {
            var movie = mapper.Map<Movie>(createMovieDto);
            if (createMovieDto.Poster is not null)
            {
                var url = await fileStorage.Store(container, createMovieDto.Poster);
                movie.Poster = url;
            }
            var id = await repo.Create(movie);
            await outCacheStore.EvictByTagAsync("movies-get", default); // Tag for Evict
            var movieDto = mapper.Map<MovieDTO>(movie);
            return TypedResults.Created($"/movies/{id}", movieDto);
        }

        static async Task<Results<Ok<MovieDTO>, NotFound>> GetById(int id, IMoviesRepository repo, IMapper mapper)
        {
            var movie = await repo.GetById(id);
            if (movie is null)
            {
                return TypedResults.NotFound();
            }
            var movieDto = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(movieDto);
        }

        static async Task<Ok<List<MovieDTO>>> GetByTitle(string title, IMoviesRepository repo, IMapper mapper)
        {
            var movies = await repo.GetByTitle(title);
            var moviesDto = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(moviesDto);
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm]CreateMovieDTO createMovieDto, IMoviesRepository repo, 
            IFileStorage fileStorage, IOutputCacheStore cacheStore, IMapper mapper)
        {
            // find local record
            var movie = await repo.GetById(id);
            if (movie is null)
            {
                return TypedResults.NotFound();
            }
            var movieForUpdate = mapper.Map<Movie>(createMovieDto);
            movieForUpdate.Id = id;
            movieForUpdate.Poster = movie.Poster;

            // user wants to update a poster
            if (createMovieDto.Poster is not null)
            {
                var url = await fileStorage.Edit(movieForUpdate.Poster, container, createMovieDto.Poster);
                movieForUpdate.Poster = url;
            }

            await repo.Update(movieForUpdate);
            await cacheStore.EvictByTagAsync("movies-get", default);

            return TypedResults.NoContent(); // Http status code 204
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IMoviesRepository repo,
            IFileStorage fileStorage, IOutputCacheStore cacheStore)
        {
            // find local record
            var movie = await repo.GetById(id);
            if (movie is null)
            {
                return TypedResults.NotFound();
            }
            await repo.Delete(id);
            await fileStorage.Delete(movie.Poster, container);
            await cacheStore.EvictByTagAsync("movies-get", default);

            return TypedResults.NoContent();
        }
    }
}
