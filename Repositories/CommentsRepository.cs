using Microsoft.EntityFrameworkCore;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    public class CommentsRepository(AppDbCtx dbCtx) : ICommentsRepository
    {
        public async Task<int> Create(Comment comment)
        {
            dbCtx.Add(comment);
            await dbCtx.SaveChangesAsync();
            return comment.Id;
        }

        public async Task Delete(int id)
        {
            await dbCtx.Comments.Where(c => c.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await dbCtx.Comments.AnyAsync(c => c.Id == id);
        }

        public async Task<List<Comment>> GetAll(int movieId)
        {
            return await dbCtx.Comments.Where(c => c.MovieId == movieId).ToListAsync();
        }

        public async Task<Comment?> GetById(int id)
        {
            return await dbCtx.Comments.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Update(Comment comment)
        {
            dbCtx.Update(comment);
            await dbCtx.SaveChangesAsync();
        }
    }
}
