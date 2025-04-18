using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.GraphQL
{
    public class Query
    {
        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Genre> GetGenres([Service] ApplicationDbContext context) => context.genres;

    }
}
