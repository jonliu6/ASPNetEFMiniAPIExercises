using MinimalAPIsWithASPNetEF.DTOs;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    /// <summary>
    /// separate class with generic, so query with pagination can be reused to multiple entities
    /// </summary>
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
        {
            // queryable skips the records from the previous page(s) and take the records from the selected page
            return queryable.Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage);
        }
    }
}
