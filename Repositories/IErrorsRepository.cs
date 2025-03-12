using MinimalAPIsWithASPNetEF.Entities;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    public interface IErrorsRepository
    {
        Task create(CustomError err);
    }
}