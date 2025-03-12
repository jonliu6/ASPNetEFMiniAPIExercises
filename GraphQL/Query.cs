using MinimalAPIsWithASPNetEF.Entities;

namespace MinimalAPIsWithASPNetEF.GraphQL
{
    public class Query
    {
        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Genre> GetGenres([Service] AppDbCtx ctx) => ctx.Genres;

        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Movie> GetMovies([Service] AppDbCtx ctx) => ctx.Movies;

        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Actor> GetActors([Service] AppDbCtx ctx) => ctx.Actors;
    }
}
