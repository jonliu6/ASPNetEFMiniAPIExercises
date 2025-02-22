using MinimalAPIsWithASPNetEF.Entities;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    public class ErrorsRepository(AppDbCtx ctx) : IErrorsRepository
    {
        public async Task create(Error err)
        {
            ctx.Add(err);
            await ctx.SaveChangesAsync();
        }
    }
}
