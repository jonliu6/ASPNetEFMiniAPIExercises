using MinimalAPIsWithASPNetEF.Entities;
using Error = MinimalAPIsWithASPNetEF.Entities.Error;

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
