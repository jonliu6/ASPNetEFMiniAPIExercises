using Microsoft.EntityFrameworkCore;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    public class GenresRepository(AppDbCtx dbCtx,
        IHttpContextAccessor iHttpCtxAccessor) : IGenresRepository
    {
        public async Task<int> Create(Genre genre)
        {
            dbCtx.Genres.Add(genre);
            await dbCtx.SaveChangesAsync();
            return genre.Id;
        }

        public async Task Delete(int id)
        {
            await dbCtx.Genres.Where( a => a.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await dbCtx.Genres.AnyAsync(a => a.Id == id);
        }

        /// <summary>
        /// used for validation rules
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> Exists(int id, string name)
        {
            return await dbCtx.Genres.AnyAsync(a => a.Id != id && a.Name == name); // check Genre name exists with a different id already
        }

        public async Task<List<Genre>> GetAll(PaginationDTO pagination)
        {
            var queryable = dbCtx.Genres.AsQueryable();
            await iHttpCtxAccessor.HttpContext!.InsertPaginationParameterInResponseHeader(queryable);
            return await queryable.OrderBy(g => g.Name).Paginate(pagination).ToListAsync();
        }

        public async Task<Genre?> GetById(int id)
        {
            return await dbCtx.Genres.AsNoTracking().FirstOrDefaultAsync (a => a.Id == id);
        }

        public async Task Update(Genre genre)
        {
            dbCtx.Genres.Update(genre);
            await dbCtx.SaveChangesAsync();
        }
    }
}
