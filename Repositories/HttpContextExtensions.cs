using Microsoft.EntityFrameworkCore;

namespace MinimalAPIsWithASPNetEF.Repositories
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// use generic in this separate class for the reusability to other entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async static Task InsertPaginationParameterInResponseHeader<T>(
            this HttpContext httpCtx, IQueryable<T> queryable)
        {
            if (httpCtx is null)
            {
                throw new ArgumentNullException(nameof(httpCtx));
            }
            double count = await queryable.CountAsync();
            httpCtx.Response.Headers.Append("totalNumberOfRecords", count.ToString());
        }
    }
}
