using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Entities;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genre genre);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string name);
        Task<List<Genre>> GetAll(PaginationDTO pagination);
        Task<Genre?> GetById(int id);
        Task Update(Genre genre);
    }
}
