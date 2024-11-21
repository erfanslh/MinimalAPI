namespace MinimalAPIMoviez.DTOs
{
    public class CreateMovieDTO
    {
        public string Title { get; set; } = null!;
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IFormFile? CoverImage { get; set; }
    }
}
