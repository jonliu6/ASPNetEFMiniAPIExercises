using MinimalAPIsWithASPNetEF.Entities;
using Error = MinimalAPIsWithASPNetEF.Entities.Error;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    public interface IErrorsRepository
    {
        Task create(Error err);
    }
}