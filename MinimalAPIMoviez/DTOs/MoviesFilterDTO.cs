using Hl7.Fhir.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MinimalAPIMoviez.Utilities;

namespace MinimalAPIMoviez.DTOs
{
    public class MoviesFilterDTO
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationDTO PaginationDTO
        { get
            {
                return new PaginationDTO { Page = Page, RecordsPerPage = RecordsPerPage };
            } 
        }

        public string? Title { get; set; }
        public int GenreId { get; set; }
        public bool InCinema { get; set; }
        public bool FutureReleases { get; set; }
        public string? OrderByField { get; set; }
        public bool OrderByAscending { get; set; } = true;

        public static ValueTask<MoviesFilterDTO> BindAsync(HttpContext context)
        {
            var page = context.ExtractValueOrDefault(nameof(Page), PaginationDTO.PageInitialValue);
            var recordsPerPage = context.ExtractValueOrDefault(nameof(RecordsPerPage), PaginationDTO.RecordsPerPageInitialValue);

            var title = context.ExtractValueOrDefault(nameof(Title),string.Empty);
            var genreId = context.ExtractValueOrDefault(nameof(GenreId), 0);
            var inTheater = context.ExtractValueOrDefault(nameof(InCinema), false);
            var futureReleases = context.ExtractValueOrDefault(nameof(FutureReleases), false);
            var orderByField= context.ExtractValueOrDefault(nameof(OrderByField), string.Empty);
            var orderByAscending = context.ExtractValueOrDefault(nameof(OrderByAscending), true);

            var response = new MoviesFilterDTO
            {
                Page = page,
                RecordsPerPage = recordsPerPage,
                Title = title,
                GenreId = genreId,
                InCinema = inTheater,
                FutureReleases = futureReleases,
                OrderByField = orderByField,
                OrderByAscending = orderByAscending,

            };
            return ValueTask.FromResult(response);
        }
    }
}
