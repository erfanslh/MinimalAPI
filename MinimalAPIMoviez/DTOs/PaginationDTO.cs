using Microsoft.IdentityModel.Tokens;
using MinimalAPIMoviez.Utilities;

namespace MinimalAPIMoviez.DTOs
{
    public class PaginationDTO
    {
        private const int PageInitialValue = 1;
        private const int RecordsPerPageInitialValue = 10;

        public int Page { get; set; } = 1;
        private int recordsPerPage { get; set; } = 10;
        private readonly int TotalRecordsPerPage = 50;

        public int RecordsPerPage
        {
            get { return recordsPerPage; }
            set
            {
                if (value > TotalRecordsPerPage)
                {
                    recordsPerPage = TotalRecordsPerPage;
                }
                else 
                {
                    recordsPerPage = value;
                }
            }
        }
        public static ValueTask<PaginationDTO> BindAsync(HttpContext httpContext)
        {
            // Query[nameof(Page)] == Query["Page"]
            var page = httpContext.ExtractValueOrDefault(nameof(Page), PageInitialValue);
            var recordsPerPage = httpContext.ExtractValueOrDefault(nameof(RecordsPerPage), RecordsPerPageInitialValue);

            var response = new PaginationDTO
            {
                Page = page,
                RecordsPerPage = recordsPerPage
            };
            return ValueTask.FromResult(response);
        }
    }
}
