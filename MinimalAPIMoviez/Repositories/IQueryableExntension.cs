using MinimalAPIMoviez.DTOs;

namespace MinimalAPIMoviez.Repositories
{
    public static class IQueryableExntension
    {
        public static IQueryable<T> Pagination<T> (this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.RecordsPerPage)
                .Take(paginationDTO.RecordsPerPage);
        }
    }
}
