namespace MinimalAPIMoviez.DTOs
{
    public class PaginationDTO
    {
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
    }
}
