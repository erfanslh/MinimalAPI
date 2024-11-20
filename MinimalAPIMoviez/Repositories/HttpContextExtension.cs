using Microsoft.EntityFrameworkCore;

namespace MinimalAPIMoviez.Repositories
{
    public static class HttpContextExtension
    {
        public async static Task InsertPaginationInResponseHeader<T>
                (this HttpContext httpContext, IQueryable<T> queryable)
        {
            if ( httpContext == null)
            {
                throw new ArgumentNullException (nameof ( httpContext));
            }
            var count = await queryable.CountAsync();
            httpContext.Response.Headers.Append("total-amount-of-arguments", count.ToString());
        }
    }
}
