using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    public class MoviesRepository(AppDbCtx dbCtx,
        IHttpContextAccessor iHttpCtxAccessor, IMapper mapper) : IMoviesRepository
    {
        public async Task<int> Create(Movie movie)
        {
            dbCtx.Movies.Add(movie);
            await dbCtx.SaveChangesAsync();
            return movie.Id;
        }

        public async Task Delete(int id)
        {
            await dbCtx.Movies.Where(m => m.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await dbCtx.Movies.AnyAsync(m => m.Id == id);
        }

        public async Task<List<Movie>> GetAll(PaginationDTO pagination)
        {
            var queryable = dbCtx.Movies.AsQueryable();
            await iHttpCtxAccessor.HttpContext!.InsertPaginationParameterInResponseHeader(queryable);
            return await queryable.OrderBy(m => m.Title).Paginate(pagination).ToListAsync();
        }
        public async Task<List<Movie>> GetByTitle(string title)
        {
            return await dbCtx.Movies.Where(m => m.Title.Contains(title)).ToListAsync();
        }

        public async Task<Movie?> GetById(int id)
        {
            return await dbCtx.Movies
                .Include(m => m.ActorsMovies.OrderBy(am => am.Order))
                    .ThenInclude(am => am.Actor) // get actor information sorted by order 
                .AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task Update(Movie movie)
        {
            dbCtx.Movies.Update(movie);
            await dbCtx.SaveChangesAsync();
        }

        public async Task Assign(int id, List<ActorMovie> actors)
        {
            for (int i = 1; i <= actors.Count; i++)
            {
                actors[i - 1].Order = i;
            }

            var movie = await dbCtx.Movies.Include(m => m.ActorsMovies).FirstOrDefaultAsync(m => m.Id == id);
            if (movie is null)
            {
                throw new ArgumentException($"There's no movie with id {id}");
            }

            movie.ActorsMovies = mapper.Map(actors, movie.ActorsMovies); // IMapper did a trick here
            await dbCtx.SaveChangesAsync();
        }
    }
}
