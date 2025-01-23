using Microsoft.EntityFrameworkCore;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    /// <summary>
    /// add IHttpContextAccessor service in Program.cs
    /// </summary>
    /// <param name="dbCtx"></param>
    /// <param name="iHttpCtxAccessor"></param>
    public class ActorsRepository(AppDbCtx dbCtx, 
        IHttpContextAccessor iHttpCtxAccessor) : IActorsRepository
    {
        /// <summary>
        /// return a list of Actors
        /// </summary>
        /// <returns></returns>
        public async Task<List<Actor>> GetAll(PaginationDTO pagination)
        {
            var queryable = dbCtx.Actors.AsQueryable();
            await iHttpCtxAccessor.HttpContext!.InsertPaginationParameterInResponseHeader(queryable);
            return await queryable.OrderBy(a => a.Name).Paginate(pagination).ToListAsync();
        }

        /// <summary>
        /// return nullable Actor object by the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Actor?> GetById(int id)
        {
            // AsNoTracking() allows to get a record from DB and do not do tracking, when update the object and save changes, the changes will not update to the record.
            return await dbCtx.Actors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Actor>> GetByName(string name)
        {
            return await dbCtx.Actors.Where(a => a.Name.Contains(name)).ToListAsync();
        }

        /// <summary>
        /// return int of actor ID
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public async Task<int> Create(Actor actor)
        {
            dbCtx.Actors.Add(actor); // or just dbCtx.Add(actor)
            await dbCtx.SaveChangesAsync();
            return actor.Id;
        }

        /// <summary>
        /// return bool indicating for existing or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Exists(int id)
        {
            return await dbCtx.Actors.AnyAsync(a => a.Id == id);
        }

        /// <summary>
        /// not return any
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public async Task Update(Actor actor)
        {
            dbCtx.Actors.Update(actor);
            await dbCtx.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await dbCtx.Actors.Where(a => a.Id == id).ExecuteDeleteAsync();
        }

    }
}
